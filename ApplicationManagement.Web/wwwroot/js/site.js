function confirmDelete(url) {
    Swal.fire({
        title: 'Are you sure?',
        text: "Record will be deleted.",
        icon: 'warning',
        showCancelButton: true
    })
        .then((result) => {

            if (result.isConfirmed) {
                window.location.href = url;
            }

        });
}