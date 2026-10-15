(function ($, window) {

    Payments = {

        that: null,

        init: function (data) {
            this.data = data;
            that = this;
            $("button.payment-button").addClass("disabled");

            fetch(data.paymentIntentUrl,
                {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json"
                    },
                    body: JSON.stringify(data.orderData)
                })
                .then(function (result) {
                    return result.json();
                })
                .then(function (data) {
                    return that.setupElements(data);
                })
                .then(function ({ stripe, card, clientSecret }) {
                    $("button.payment-button").removeClass("disabled");
                    var $form = $("#payment-form");
                    $form.on("submit",
                        function (event) {
                            event.preventDefault();
                            if ($(this).valid()) {
                                that.pay(stripe, card, clientSecret);
                            }
                        });
                });
        },

        setupElements: function (data) {
            var stripe = Stripe(data.PublishableKey);
            var elements = stripe.elements({
                locale: "auto"
            });

            var style = {
                base: {
                    "::placeholder": {
                        color: "#aab7c4"
                    }
                },
                invalid: {
                    color: "#fa755a",
                    iconColor: "#fa755a"
                }
            };

            var card = elements.create("card", { style: style });
            card.mount("#card-element");

            var paymentRequest = stripe.paymentRequest({
                country: "US",
                currency: "usd",
                total: {
                    label: this.data.orderData.description,
                    amount: this.data.orderData.amountInCents
                },
                requestPayerName: true,
                requestPayerEmail: true
            });

            var prButton = elements.create("paymentRequestButton", {
                paymentRequest: paymentRequest
            });

            paymentRequest.canMakePayment().then(function (result) {
                if (result) {
                    prButton.mount("#payment-request-button");
                    $("#enter-payment-details").show();
                    $("#enter-payment-details-separator").show();
                } else {
                    document.getElementById("payment-request-button").style.display = "none";
                }
            });

            paymentRequest.on("paymentmethod", function (ev) {
                stripe.confirmCardPayment(
                    data.ClientSecret,
                    { payment_method: ev.paymentMethod.id },
                    { handleActions: false }
                ).then(function (confirmResult) {
                    if (confirmResult.error) {
                        ev.complete("fail");
                    } else {
                        ev.complete("success");
                        if (confirmResult.paymentIntent.status === "requires_action") {
                            stripe.confirmCardPayment(data.ClientSecret).then(function (result) {
                                if (result.error) {
                                    that.processError(result.error.message, true);
                                } else {
                                    that.orderComplete(stripe, data.ClientSecret, ev.payerName, ev.payerEmail);
                                }
                            });
                        } else {
                            that.orderComplete(stripe, data.ClientSecret, ev.payerName, ev.payerEmail);
                        }
                    }
                });
            });

            return {
                stripe: stripe,
                card: card,
                clientSecret: data.ClientSecret
            };
        },

        pay: function (stripe, card, clientSecret) {
            this.changeLoadingState(true);

            stripe
                .confirmCardPayment(clientSecret,
                    {
                        payment_method: {
                            card: card
                        }
                    })
                .then(function (result) {
                    if (result.error) {
                        that.processError(result.error.message, true);
                    } else {
                        that.orderComplete(stripe, clientSecret);
                    }
                });
        },

        processPaymentInformation: function (paymentIntent, name, email) {
            var errorMessage = "There was an error processing your request. Please contact support";
            var fullName = name || $("input[name='FullName']").val();
            var emailAddress = email || $("input[name='EmailAddress']").val();
            var phoneNumber = $("input[name='PhoneNumber']").val();

            that.changeLoadingState(true);

            ajaxcontroller().send(that.data.processPaymentUrl,
                {
                    id: that.data.orderData.id,
                    quantity: that.data.orderData.quantity,
                    paymentIntentId: paymentIntent.id,
                    fullName: fullName,
                    emailAddress: emailAddress,
                    phoneNumber: phoneNumber
                }, "POST").done(
                    function (result) {
                        if (result.data.success) {
                            if (that.data.postPaymentUrl) {
                                ajaxcontroller().send(that.data.postPaymentUrl,
                                    {
                                        purchaseModel: result.data.purchaseModel,
                                    }, "POST").done(
                                        function (result) {
                                            if (result.data.success) {
                                                document.location = that.data.successUrl;
                                            } else {
                                                that.changeLoadingState(false);
                                                that.processError(result.errorMessage || errorMessage);
                                            }
                                        });
                            } else {
                                document.location = that.data.successUrl;
                            }
                        } else {
                            that.changeLoadingState(false);
                            that.processError(result.errorMessage || errorMessage);
                        }
                    });
        },

        orderComplete: function (stripe, clientSecret, name, email) {
            stripe.retrievePaymentIntent(clientSecret).then(function (result) {
                var paymentIntent = result.paymentIntent;
                var paymentIntentJson = JSON.stringify(paymentIntent, null, 2);

                if (that.data.successUrl) {
                    that.processPaymentInformation(paymentIntent, name, email);
                } else {
                    that.changeLoadingState(false);

                    $(".sr-payment-form").addClass("hidden");
                    $("pre").textContent = paymentIntentJson;

                    $(".sr-result").removeClass("hidden");
                    setTimeout(function () {
                        $(".sr-result").addClass("expand");
                    },
                        200);
                }
            });
        },

        processError: function (errorMsgText, showError) {
            this.changeLoadingState(false);

            if (!showError && that.data.failureUrl) {
                document.location = that.data.failureUrl + "?errorMessage=" + errorMsgText;
            } else {
                var errorMsg = $(".sr-field-error");
                errorMsg.textContent = errorMsgText;
            }
        },

        changeLoadingState: function (isLoading) {
            var $buttonText = $("#button-text");
            var $spinner = $("#spinner");

            if (isLoading) {
                $spinner.removeClass("hidden");
                $buttonText.addClass("hidden");
                $("#pageSpinner").show();
                $("#pageOverlay").show();

            } else {
                $spinner.addClass("hidden");
                $buttonText.removeClass("hidden");
                $("#pageSpinner").hide();
                $("#pageOverlay").hide();
            }
        }
    };

})(jQuery, this)