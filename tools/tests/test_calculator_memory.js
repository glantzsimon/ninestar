// Offline behaviour checks. No network, browser storage or website requests.
const assert = require('assert');
const fs = require('fs');
const vm = require('vm');
const path = require('path');
const source = fs.readFileSync(path.join(__dirname, '../../webapp/WebApplication/Scripts/k9/calculatorMemory.js'), 'utf8');

function field(name, value, type = 'text', extra = {}) {
    return Object.assign({ name, value, type, checked: false, attrs: {} }, extra);
}

function run(page, inputs, storage, restore = true, displays = {}, hidden = false) {
    const triggered = [];
    const form = { inputs, attrs: { 'data-calculator-page': page, 'data-calculator-restore': String(restore), 'data-calculator-hidden-inputs': String(hidden) }, events: {} };
    function collection(items) {
        return {
            length: items.length,
            each(fn) { items.forEach((e, i) => fn.call(e, i, e)); return this; },
            filter(fn) {
                return collection(items.filter(e => typeof fn === 'function' ? fn.call(e) :
                    e.name === 'TimeOfBirthKnown' || e.name === 'DisplayDataForPeriod'));
            },
            find(selector) {
                const all = items[0].inputs || [];
                if (selector === 'input, select') return collection(all);
                if (selector === '.calculator-options input:checked') return collection(all.filter(e => e.name === 'CalculatorType' && e.checked));
                if (selector === '#CalculationMethod') return collection(all.filter(e => e.name === 'CalculationMethod'));
                return collection([]);
            },
            attr(key) { return items[0].attrs[key]; },
            val(value) {
                if (!arguments.length) return items[0] && items[0].value;
                items.forEach(e => { e.value = value; }); return this;
            },
            closest() { return collection(items); },
            data() { return items[0] && items[0].picker; },
            trigger() { items.forEach(e => triggered.push(e.name)); return this; },
            toggle() { return this; },
            on(events, selector, handler) { items[0].events[events] = handler || selector; return this; }
        };
    }
    function $(value) { return collection(typeof value === 'string' ? [form] : [value]); }
    $.fn = {};
    const context = { $, localStorage: storage, document: { getElementById: id => displays[id] }, moment: value => value };
    vm.runInNewContext(source, context);
    context.calculatorMemory().init();
    return { form, triggered, save: () => form.events['submit.calculatorMemory']() };
}

const data = new Map();
const storage = { getItem: k => data.get(k) || null, setItem: (k, v) => data.set(k, v) };
const key = page => 'NineStarKi.CalculatorInputs.v1:' + page;

const saved = run('personal-chart', [
    field('PersonModel.DateOfBirth', '1979-06-16', 'text', { attrs: { linkid: 'old-id' } }),
    field('PersonModel.TimeOfBirth', '07:00', 'time'),
    field('PersonModel.Gender', '1', 'select-one'),
    field('TimeOfBirthKnown', 'true', 'checkbox', { checked: true }),
    field('TimeOfBirthKnown', 'false', 'hidden'),
    field('CalculatorType', '0', 'radio'),
    field('CalculatorType', '1', 'radio', { checked: true }),
    field('CalculationMethod', '1', 'select-one'),
    field('IsAdmin', 'true'),
    field('__RequestVerificationToken', 'secret', 'hidden'),
    field('PersonModel.Name', 'secret', 'password'),
    field('random-date-picker-id', '16/06/1979')
], storage);
assert.equal(data.size, 0, 'Rendering must not overwrite saved inputs');
saved.save();
const values = JSON.parse(data.get(key('personal-chart'))).values;
assert.equal(values.TimeOfBirthKnown, true, 'Hidden MVC checkbox fallback must not overwrite checkbox');
assert.equal(values.CalculatorType, '1', 'Only the selected radio value is stored');
assert.equal(values['PersonModel.DateOfBirth'], '1979-06-16');
assert.equal(values['PersonModel.TimeOfBirth'], '07:00');
assert.deepEqual(Object.keys(values).sort(), ['CalculationMethod', 'CalculatorType', 'PersonModel.DateOfBirth', 'PersonModel.Gender', 'PersonModel.TimeOfBirth', 'TimeOfBirthKnown'].sort());

let restoredDate;
const restoredFields = [
    field('PersonModel.DateOfBirth', '2000-01-01', 'text', { attrs: { linkid: 'new-id' } }),
    field('PersonModel.TimeOfBirth', '00:00', 'time'),
    field('TimeOfBirthKnown', 'true', 'checkbox'),
    field('CalculationMethod', '0', 'select-one'),
    field('CalculatorType', '0', 'radio', { checked: true }),
    field('CalculatorType', '1', 'radio')
];
const restored = run('personal-chart', restoredFields, storage, true, { 'new-id': { picker: { date: value => { restoredDate = value; } } } });
assert.equal(restoredDate, '1979-06-16', 'Date must restore through the newly generated picker ID');
assert.equal(restoredFields[0].value, '1979-06-16');
assert.equal(restoredFields[1].value, '07:00');
assert.equal(restoredFields[2].checked, true);
assert.equal(restoredFields[4].checked, false);
assert.equal(restoredFields[5].checked, true);
assert.deepEqual(restored.triggered, ['TimeOfBirthKnown'], 'Restoration must not trigger preference-writing change handlers');
assert.ok(restored.form.events['dp.change.calculatorMemory'], 'Date-picker changes must be saved');

const other = field('PersonModel.DateOfBirth', '1982-03-12');
run('predictions', [other], storage);
assert.equal(other.value, '1982-03-12', 'Calculator pages must not share values');
const posted = field('PersonModel.DateOfBirth', '1990-01-01');
run('personal-chart', [posted], storage, false);
assert.equal(posted.value, '1990-01-01', 'Do not overwrite a POST response or server validation errors');

const pair = run('compatibility', [field('NineStarKiModel1.PersonModel.Name', 'Alex'), field('NineStarKiModel2.PersonModel.Name', 'Jamie')], storage);
pair.save();
const pairFields = [field('NineStarKiModel1.PersonModel.Name', ''), field('NineStarKiModel2.PersonModel.Name', '')];
run('compatibility', pairFields, storage);
assert.deepEqual(pairFields.map(e => e.value), ['Alex', 'Jamie']);
const linked = run('numerology', [field('PersonModel.DateOfBirth', '16/06/1979', 'hidden', { attrs: { 'data-calculator-date': '1979-06-16' } }), field('__RequestVerificationToken', 'secret', 'hidden')], storage, false, {}, true);
linked.save();
assert.deepEqual(JSON.parse(data.get(key('numerology'))).values, { 'PersonModel.DateOfBirth': '1979-06-16' }, 'Result-page links preserve ISO dates without storing anti-forgery tokens');
data.set(key('numerology'), '{broken json');
assert.doesNotThrow(() => run('numerology', [field('PersonModel.Name', 'unchanged')], storage));
const unavailable = { getItem() { throw Error('disabled'); }, setItem() { throw Error('full'); } };
assert.doesNotThrow(() => run('biorhythms', [field('PersonModel.Name', 'Alex')], unavailable).save());
console.log('Calculator memory behaviour checks passed.');
