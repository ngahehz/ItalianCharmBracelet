async function checkVoucher() {
    try {
        const response = await fetch('/Checkout/CheckVoucherAvailability', {
            method: 'GET',
        });
        if (!response.ok) throw new Error('Lỗi khi gọi API');

        const data = await response.json();
        return data;
    } catch (error) {
        console.error('Lỗi kiểm tra voucher:', error);
        return false;
    }
}

    const form = document.getElementById('formCheckout');

    paypal.Buttons({
        style: {
            layout: 'vertical',
            color: 'gold',
            tagline: 'false'
        },

        onClick: function (data, actions) {
            if (!form.checkValidity()) {
                form.reportValidity();
                return Promise.reject();
            }

            return fetch('/Checkout/CheckVoucherAvailability', {
                method: 'get',
                headers: {
                    'content-type': 'application/json'
                }
            }).then(function (res) {
                return res.json();
            }).then(function (data) {
                if (!form.checkValidity()) {
                    form.reportValidity();
                    alert("Ddien thong tin day du di");

                    return actions.reject();
                }
                else if (data.success == false) {
                    alert(data.message);
                    return actions.reject();
                }
                else {
                    return actions.resolve();
                }
            });
        },

        async createOrder() {
            try {
                const response = await fetch("/Checkout/create-paypal-order", {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                    }
                });

                if (!response.ok) {
                    throw new Error(`Server error: ${response.status} ${response.statusText}`);
                }

                const order = await response.json();

                if (!order.id || order.id.trim() === "") {
                    throw new Error("Invalid order ID received from server.");
                }

                return order.id;
            } catch (error) {
                console.error("Error in createOrder:", error);
                alert("An error occurred while creating the PayPal order. Please try again.");
                throw error;
            }
            // const response = await fetch("/Cart/create-paypal-order", {
            //     method: "POST",
            //     headers: {
            //         "Content-Type": "application/json",
            //     }
            // });

            // const order = await response.json();
            // return order.id;
        },

        async onApprove(data) {
            try {
                const paymentMethod = document.getElementById('paypal-button-container').getAttribute('data-method');

                const formData = new FormData(form);
                const jsonObject = {};

                formData.forEach((value, key) => {
                    jsonObject[key] = value;
                });

                jsonObject.PaymentMethod = paymentMethod;
                jsonObject.OrderId = data.orderID;

                const response = await fetch(`/Checkout/capture-paypal-order`, {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                    },
                    body: JSON.stringify(jsonObject)
                });

                const details = await response.json();

                if (details === "success") {
                    window.location.href = "/Checkout/PaymentSuccess";
                } else {
                    alert("Transaction not completed");
                }
            }
            catch (error) {
                console.error("Error during onApprove:", error);
                alert("An error occurred during payment processing. Please try again.");
            }
        }
    }).render('#paypal-button-container');


