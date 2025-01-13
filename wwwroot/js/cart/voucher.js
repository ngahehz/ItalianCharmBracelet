var flag_code = false;
function addVoucher(subtotal) {
    code = document.getElementById('code_promotion').value;
    var message = document.getElementById('error_promotion');

    $.ajax({
        url: '/Cart/AddVoucher',
        type: 'POST',
        data: {
            code: code,
            Subtotal: subtotal
        },
        success: function (response) {
            var element = document.getElementById('promotion');

            if (response.success) {
                if (element) {
                    element.textContent = toMoneyFormat(response.promotion) + " VNĐ";
                    message.textContent = "Bạn áp thành công mã '" + response.message + "'";
                    flag_code = true;
                }
            }
            else {
                message.textContent = response.message;
                element.textContent = "";
            }
        },
        error: function () {
            alert("Có lỗi xảy ra.");
        }
    })
}

function checkFlagCode() {
    flag_code = false;
    document.getElementById('promotion').textContent = "";
}

async function initiateVoucher(subtotal) {
    const isAuthenticated = document.getElementById('btnPay').getAttribute('data-is-authenticated') === 'True';
    if (isAuthenticated) {
        addVoucher(subtotal);
    } else {
        showLoginModal('/Cart')
    }
}

