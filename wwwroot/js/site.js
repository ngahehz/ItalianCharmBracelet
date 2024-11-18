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


function showLoginModal() {
    $('#registerContent').addClass('hidden'); // Ẩn Register
    $('#loginContent').removeClass('hidden'); // Hiển thị Login
    $('#myModalLabel').text('Login');
    $('#myModal').modal('show');
}

function showRegisterModal() {
    $('#loginContent').addClass('hidden'); // Ẩn Login
    $('#registerContent').removeClass('hidden'); // Hiển thị Register
    $('#myModalLabel').text('SignUp');
}

$(document).on("submit", "#loginForm", function (e) {
    alert("1")
    e.preventDefault();
    $.ajax({
        url: "/Customer/Login",
        type: "POST",
        data: $(this).serialize(),
        success: function (response) {
            if (response.success) {
                alert("2")
                location.reload();
                //window.location.href = response.returnUrl;
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
    alert("1")
    e.preventDefault();
    $.ajax({
        url: "/Customer/Register",
        type: "POST",
        data: $(this).serialize(),
        success: function (response) {
            if (response.success) {
                alert("2")
                alert(response.message);
                location.reload();
            } else {
                alert("3")
                alert(response.errors)
                $("#registerContent").html(response); // Update modal with errors if login failed
            }
        },
        error: function () {
            alert("An error occurred. Please try again.");
        }
    });
});