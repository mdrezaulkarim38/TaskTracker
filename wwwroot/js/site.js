function showAlert(type, message, icon = 'success') {
    if (type === 'toast') {
        Swal.fire({
            toast: true,
            position: 'top-end',
            icon: icon,
            title: message,
            showConfirmButton: false,
            timer: 2500,
            timerProgressBar: true
        });
    } else {
        Swal.fire({
            icon: icon,
            title: message,
            confirmButtonText: 'OK'
        });
    }
} 