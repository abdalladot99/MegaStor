function showAlert(itemId,stringe) {
    const swalWithBootstrapButtons = Swal.mixin({
        customClass: {
            confirmButton: 'btn btn-danger mx-3',
            cancelButton: 'btn btn-light'
        },
        buttonsStyling: false
    });

    swalWithBootstrapButtons.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "Yes, delete it!",
        cancelButtonText: "No, cancel!",
        reverseButtons: true
    }).then((result) => {
        if (result.isConfirmed) {
            // إرسال الطلب باستخدام AJAX
            $.ajax({
                url: stringe, // رابط الـ Controller
                type: 'POST',
                data: { id: itemId }, // إرسال الـ ID
                success: function () {
                    swalWithBootstrapButtons.fire({
                        title: "Deleted!",
                        text: "Your file has been deleted.",
                        icon: "success"
                    }).then(() => {
                        location.reload(); // تحديث الصفحة
                    });
                },
                error: function () {
                    swalWithBootstrapButtons.fire({
                        title: "Error!",
                        text: "An error occurred while deleting.",
                        icon: "error"
                    }).then(() => {
                        location.reload(); // تحديث الصفحة
                    });
                }
            });
        } else if (result.dismiss === Swal.DismissReason.cancel) {
            swalWithBootstrapButtons.fire({
                title: "Cancelled",
                text: "Your item is safe.",
                icon: "error"
            });
        }
    });
}

function DeleteNow(itemId, url) {
    $.ajax({
        url: url,
        type: 'POST',
        data: { id: itemId },
        success: function () {
            location.reload(); // إعادة تحميل الصفحة بعد الحذف
        },
        error: function (xhr, status, error) {
            console.error("Error deleting:", error);
            alert("حدث خطأ أثناء الحذف.");
        }
    });
}

