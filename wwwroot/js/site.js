// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


function updateCart(productId, productQuantity = 1) {
    $.ajax({
        url: '/Cart/UpdateCart',
        type: 'POST',
        data: {
            id: productId,
            quantity: productQuantity
        },
        success: function (response) {
            if (response.success) {
                updateCartDisplay(response.gioHang);
                var element = document.getElementById('total-' + productId);
                if (element) {
                    element.textContent = toMoneyFormat(response.total);
                }
            }
            else if (response.remove) {
                removeFromCart(productId);
            }
            else {
                alert("looix"); // Thông báo lỗi nếu có
            }
        },
        error: function () {
            alert("Có lỗi xảy ra khi thêm sản phẩm vào giỏ hàng.");
        }
    });
}

function updateCartDisplay(cart) {
    document.getElementById('cart-panel').textContent = cart.quantity;
}

function removeFromCart(productId) {
    $.ajax({
        url: '/Cart/RemoveFormCart',
        type: 'POST',
        data: { id: productId },
        success: function (response) {
            $('#row-' + productId).remove();
            updateCartDisplay(response.gioHang);
        },
        error: function (error) {
            console.error("Có lỗi xảy ra:", error);
        }
    });
}

function toMoneyFormat(price) {
    return price.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
}


function showLoginModal(returnUrl) {
    $('#registerContent').addClass('hidden');
    $('#loginContent').removeClass('hidden'); 
    $('#myModalLabel').text('Login');
    $('#myModal').modal('show');
    const element = document.getElementById("requiredlogin_returnurl");
    if (element) 
        sessionStorage.setItem('returnUrl', sessionStorage.getItem('requiredlogin_returnurl'));
     else 
        sessionStorage.setItem('returnUrl', returnUrl);
}

function showRegisterModal() {
    $('#loginContent').addClass('hidden'); 
    $('#registerContent').removeClass('hidden');
    $('#myModalLabel').text('SignUp');
}


$(document).on("submit", "#loginForm", function (e) {
    e.preventDefault();
    $.ajax({
        url: "/Customer/Login",
        type: "POST",
        data: $(this).serialize(),
        success: function (response) {
            if (response.success) {
                alert("đăng nhập thành công")
                const returnUrl = sessionStorage.getItem('returnUrl');
                if (returnUrl) {
                    window.location.href = returnUrl;

                    sessionStorage.removeItem('returnUrl');
                    sessionStorage.removeItem('requiredlogin_returnurl');
                } else {
                    location.reload();
                }
            } else {
                $("#loginContent").html(response); // Update modal with errors if login failed
            }
        },
        error: function () {
            alert("An error occurred. Please try again.");
        }
    });
});

$(document).on("submit", "#registerForm", function (e) {
    e.preventDefault();
    $.ajax({
        url: "/Customer/Register",
        type: "POST",
        data: $(this).serialize(),
        success: function (response) {
            if (response.success) {
                alert("đăng ký thành công")
                location.reload();
            } else {
                $("#registerContent").html(response); // Update modal with errors if login failed
            }
        },
        error: function () {
            alert("An error occurred. Please try again.");
        }
    });
});

async function initiatePayment() {
    const isAuthenticated = document.getElementById('btnPay').getAttribute('data-is-authenticated') === 'True';
    if (isAuthenticated) {
        window.location.href = '/Cart/Checkout';
    } else {
        showLoginModal('/Cart/Checkout')
    }
}

$(document).on("submit", "#formCheckout", function (e) {
    e.preventDefault();
    const paymentMethod = document.getElementById('COD-button').value;

    const formData = $(this).serializeArray(); 
    formData.push({ name: "PaymentMethod", value: paymentMethod });

    $.ajax({
        url: "/Cart/Checkout",
        type: "POST",
        data: formData,
        success: function (response) {
            if (response.success) {
                window.location.href = '/Cart/PaymentSuccess';
            } else {
                $("#checkout_form").html(response);
            }
        },
        error: function () {
            alert("An error occurred. Please try again.");
        }
    });
});

//document.addEventListener('DOMContentLoaded', async () => {
//    const currentPageUrl = window.location.pathname;  // Lấy URL hiện tại

//    const response = await fetch(currentPageUrl, { method: 'GET' });

//    if (response.status === 401) {
//        showLoginModal('/Cart/Checkout');
//    } else if (response.status === 200) {
//        //window.location.href = '/Cart/Checkout';
//    }
//});

//document.addEventListener('DOMContentLoaded', async () => {
//    const response = await fetch('/Cart/Checkout', { method: 'GET' });

//    if (response.status === 401) {
//        // Người dùng chưa đăng nhập, hiển thị modal đăng nhập
//        showLoginModal('/Cart/Checkout');
//    } else if (response.status === 200) {
//        // Người dùng đã đăng nhập, tiếp tục xử lý nội dung
//        window.location.href = '/Cart/Checkout';
//    }
//});

