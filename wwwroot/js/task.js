$(document).ready(function () {
    let taskIdToDelete = null;
    const token = $('input[name="__RequestVerificationToken"]').val();

    $('#statusFilter, #sortOrder').on('change', function () {
        $('#filterForm').submit();
    });

    let searchTimeout;
    $('#searchInput').on('keyup', function () {
        clearTimeout(searchTimeout);
        searchTimeout = setTimeout(function () {
            $('#filterForm').submit();
        }, 500);
    });

    $(document).on('change', '.toggle-status', function () {
        const checkbox = $(this);
        const taskId = checkbox.data('id');
        const isChecked = checkbox.is(':checked');
        const originalState = !isChecked;

        checkbox.prop('disabled', true);

        $.ajax({
            url: '/Task/ToggleStatus',
            type: 'POST',
            data: {
                id: taskId,
                __RequestVerificationToken: token
            },
            success: function (response) {
                if (response.success) {
                    const label = $(`#status-label-${taskId}`);
                    label.text(response.isCompleted ? 'Completed' : 'Pending');

                    const row = checkbox.closest('tr');
                    row.css('backgroundColor', '#d4edda');
                    setTimeout(() => row.css('backgroundColor', ''), 1000);
                } else {
                    checkbox.prop('checked', originalState);
                    alert(response.message || 'Error updating task status');
                }
            },
            error: function () {
                checkbox.prop('checked', originalState);
                alert('Error updating task status.');
            },
            complete: function () {
                checkbox.prop('disabled', false);
            }
        });
    });

    $(document).on('click', '.delete-btn', function () {
        taskIdToDelete = $(this).data('id');
        const taskTitle = $(this).data('title');

        $('#deleteTaskTitle').text(taskTitle);
        $('#deleteModal').modal('show');
    });

    $('#confirmDelete').on('click', function () {
        if (!taskIdToDelete) return;

        const button = $(this);
        button.prop('disabled', true).text('Deleting...');

        $.ajax({
            url: '/Task/Delete',
            type: 'POST',
            data: {
                id: taskIdToDelete,
                __RequestVerificationToken: token
            },
            success: function () {
                location.reload();
            },
            error: function () {
                alert('Error deleting task.');
            },
            complete: function () {
                button.prop('disabled', false).text('Delete');
            }
        });
    });

    setTimeout(function () {
        $('.alert').fadeOut('slow');
    }, 3000);
});