async function checkVoucher() {
    try {
        const response = await fetch('/Cart/CheckVoucherAvailability', {
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

        onInit(data, actions) {
            actions.disable();
            form.addEventListener('input', async function (event) {
                if (!form.checkValidity()) {
                    actions.disable();
                }
                else {
                    const result = await checkVoucher();
                    if (result.success == false) {
                        actions.disable();
                        alert(actions.message);
                    } else {
                        actions.enable();
                        //temp = result.discount;
                    }
                }
            });
        },

        onClick() {
            if (!form.checkValidity()) {
                form.reportValidity();
            }
        },

        async createOrder() {
            try {
                const response = await fetch("/Cart/create-paypal-order", {
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

                const response = await fetch(`/Cart/capture-paypal-order`, {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                    },
                    body: JSON.stringify(jsonObject)
                });

                const details = await response.json();

                if (details === "success") {
                    window.location.href = "/Cart/PaymentSuccess";
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


