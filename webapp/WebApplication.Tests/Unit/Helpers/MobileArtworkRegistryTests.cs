using K9.WebApplication.Helpers;
using K9.WebApplication.Models;
using K9.WebApplication.Enums;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using Xunit;

namespace K9.WebApplication.Tests.Unit.Helpers
{
    public class MobileArtworkRegistryTests
    {
        [Fact]
        public void CatalogueIsPublicCompleteAndDeduplicatesIdenticalImageContent()
        {
            var data = MobileArtworkRegistry.Create("https://media.example/Images");
            Assert.Equal(424, data.Assets.Count);
            var variants = data.Assets.Values.SelectMany(e => e.Variants.Values).ToArray();
            Assert.Equal(431, variants.Length);
            Assert.Equal(424, variants.Select(e => e.Url).Distinct().Count());
            Assert.All(variants.GroupBy(e => e.Revision), sameImage =>
                Assert.Single(sameImage.Select(e => e.Url).Distinct()));
            Assert.Equal(data.Assets["ninestar.elements.earth"].Variants["default"].Url,
                data.Assets["ninestar.modalities.stable"].Variants["default"].Url);
            foreach (var item in variants)
            {
                Assert.True(item.Width > 0 && item.Height > 0);
                Assert.Equal(64, item.Revision.Length);
                Assert.StartsWith("https://media.example/Images/", item.Url);
                Assert.Contains("?v=" + item.Revision, item.Url);
                Assert.DoesNotContain("userinfos/", item.Url);
                Assert.DoesNotContain("articles/", item.Url);
                Assert.StartsWith("image/", item.ContentType);
            }
        }

        [Fact]
        public void ThemeVariantsShareAnIdAndPreserveOriginalFilenameCase()
        {
            var data = MobileArtworkRegistry.Create("https://media.example/Images/");
            Assert.Equal(new[] { "dark", "default" }, data.Assets["iching.iching"].Variants.Keys.OrderBy(e => e).ToArray());
            Assert.Equal("iching.iching", MobileArtworkRegistry.IdForPath("iching/iching-dark.png"));
            Assert.Contains("/numerology/Pioneer.png?", data.Assets["numerology.pioneer"].Variants["default"].Url);
            Assert.Null(MobileArtworkRegistry.IdForPath("userinfos/3/photo.png"));
            Assert.Null(MobileArtworkRegistry.IdForPath("../company/logo.png"));
            Assert.Null(MobileArtworkRegistry.IdForPath(null));
        }

        [Fact]
        public void VersionChangesWithMediaFallbackAndResponseMutationsAreIsolated()
        {
            var first = MobileArtworkRegistry.Create("https://media.example/Images");
            var repeat = MobileArtworkRegistry.Create("https://media.example/Images/");
            Assert.Equal(first.Version, repeat.Version);
            Assert.NotEqual(first.Version, MobileArtworkRegistry.Create("https://site.example/ninestar/Images").Version);
            first.Assets["iching.iching"].Variants.Clear();
            Assert.Equal(2, repeat.Assets["iching.iching"].Variants.Count);
            Assert.Equal(2, MobileArtworkRegistry.Create("https://media.example/Images").Assets["iching.iching"].Variants.Count);
        }

        [Fact]
        public void EveryEnergySymbolMoonPhaseAndNumerologyCodeResolvesToTheCatalogue()
        {
            var data = MobileArtworkRegistry.Create("https://media.example/Images");
            for (var number = 1; number <= 9; number++)
            {
                var energy = MobilePersonalChartMapper.Energy(new NineStarKiEnergy((ENineStarKiEnergy)number));
                foreach (var id in new[] { energy.ArtworkId, energy.ElementArtworkId, energy.TrigramArtworkId,
                    energy.ModalityArtworkId, energy.SeasonArtworkId, energy.YinYangArtworkId })
                {
                    Assert.NotNull(id);
                    Assert.True(data.Assets.ContainsKey(id));
                }
                var code = new NumerologyCodeDetails { NumerologyCode = (ENumerologyCode)number };
                var mapped = JObject.FromObject(MobileCalculationMapper.Code(code));
                Assert.True(data.Assets.ContainsKey((string)mapped["ArtworkId"]));
            }
            for (var illumination = 0; illumination <= 100; illumination++)
                foreach (var waxing in new[] { true, false })
                {
                    var moon = JObject.FromObject(MobileCalculationMapper.Moon(new MoonPhase(illumination, waxing)));
                    Assert.True(data.Assets.ContainsKey((string)moon["ArtworkId"]));
                }
        }

        [Theory]
        [InlineData(null)]
        [InlineData("/Images")]
        [InlineData("file:///Images")]
        [InlineData("https://media.example/Images?token=private")]
        [InlineData("https://user:password@media.example/Images")]
        public void RejectsUnsafeOrMissingMediaBase(string value)
        {
            Assert.Throws<ArgumentException>(() => MobileArtworkRegistry.Create(value));
        }

        [Theory]
        [InlineData("W/\"abc\"", true)]
        [InlineData("\"abc\"", true)]
        [InlineData("\"old\", W/\"abc\"", true)]
        [InlineData("*", true)]
        [InlineData("\"abcdef\"", false)]
        [InlineData("abc", false)]
        [InlineData("W/*", false)]
        [InlineData(null, false)]
        public void ConditionalGetUsesWholeTags(string header, bool expected)
        {
            Assert.Equal(expected, MobileArtworkRegistry.MatchesETag(header, "abc"));
        }
    }
}
