$(document).ready(function () {
    loadDataTable();
})

function loadDataTable() {
    datatable = $('#tblData').DataTable({
        "ajax": { url: '/admin/movie/getall' },
        "columns": [
            { data: 'name', "width": "25%" },
            { data: 'director', "width": "15%" },
            { data: 'duration', "width": "10%" },
            { data: 'rating', "width": "20%" },
            { data: 'category.name', "width": "15%" },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                     <a href="/admin/movie/upsert?id=${data}" class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i> Edit</a>               
                     <a onClick=deleteItem('/admin/movie/delete/${data}') class="btn btn-danger mx-2"> <i class="bi bi-trash-fill"></i> Delete</a>
                    </div>`
                },
                "width": "25%"
            }
        ]

    });
}

function deleteItem(url) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    console.log(data);
                    datatable.ajax.reload();
                    //loadDataTable();
                    //toastr.success(data.message);
                    Swal.fire({
                        title: "Deleted!",
                        text: data.message,
                        icon: "success"
                    });
                }
            })
        }
    });

}
