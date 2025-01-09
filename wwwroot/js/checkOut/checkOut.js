$(document).on("submit", "#formCheckout", function (e) {
    e.preventDefault();
    const paymentMethod = document.activeElement.value;

    const formData = $(this).serializeArray();
    formData.push({ name: "PaymentMethod", value: paymentMethod });

    $.ajax({
        url: "/Cart/Checkout",
        type: "POST",
        data: formData,
        success: function (response) {
            if (response.success) {
                window.location.href = response.redirectUrl;
            } else {
                $("#checkout_form").html(response);
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.error("Error Status: " + textStatus);
            console.error("Error Thrown: " + errorThrown);
            console.error("Response Text: " + jqXHR.responseText);
            alert("An error occurred: " + textStatus);
        }
    });
});
