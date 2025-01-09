async function initiatePayment() {
    const isAuthenticated = document.getElementById('btnPay').getAttribute('data-is-authenticated') === 'True';
    if (isAuthenticated) {
        saveVoucher()
    } else {
        showLoginModal('/Cart/Checkout')
    }
}

function saveVoucher() {
    var data = $('#code_promotion').val();
    if (!flag_code) 
        data = "";

    $.ajax({
        url: '/Cart/SaveVoucher',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({ inputData: data }),
        success: function () {
            window.location.href = '/Cart/Checkout';
        },
        error: function (xhr, status, error) {
            console.error("Lỗi khi gửi dữ liệu:", error);
        }
    });
}