    let cooldown = 60;
    let interval;
    let isPhoneVerified = false;

    function openOtpModal() {
        const phone = document.getElementById("Phone").value;
        if (!/^\d{10}$/.test(phone)) {
            alert("Enter a valid 10-digit phone number before verification.");
            return;
        }

        document.getElementById("otpPhone").value = phone;
        document.getElementById("otpInput").value = '';
        document.getElementById("otpStatus").textContent = '';
        isPhoneVerified = false;

        $('#otpModal').modal('show');
    }

    function sendOtp() {
        const phone = document.getElementById("otpPhone").value;

        fetch('/Otp/SendOtp', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ phoneNumber: phone })
        })
        /*.then(res => res.json())
        .then(data => {
            document.getElementById("otpStatus").textContent = data.message;
        });*/
        .then(async res => {
            const data = await res.json();
            if (!res.ok) {
                throw new Error(data.message || "Failed to send OTP");
            }
            document.getElementById("otpStatus").textContent = data.message;
        })
        .catch(err => {
            document.getElementById("otpStatus").textContent = "Error: " + err.message;
        });
    }

    function verifyOtp() {
        const phone = document.getElementById("otpPhone").value;
        const otp = document.getElementById("otpInput").value;

        fetch('/Otp/VerifyOtp', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ phoneNumber: phone, otpCode: otp })
        })
        .then(res => res.json())
        .then(data => {
            document.getElementById("otpStatus").textContent = data.message;

            if (data.success) {
                isPhoneVerified = true;
                $('#otpModal').modal('hide');
                alert("Phone number verified successfully.");
            }
        });
    }

    function checkOtpBeforeSubmit(event) {
        if (!isPhoneVerified) {
            event.preventDefault();
            alert("Please verify phone number via OTP before submitting.");
        }
    }

    document.addEventListener("DOMContentLoaded", function () {
        const form = document.getElementById("enroll_form");
        if (form) {
            form.addEventListener("submit", checkOtpBeforeSubmit);
        }
    });
