$(document).ready(function() {
    const token = $('#antiForgeryForm input[name="__RequestVerificationToken"]').val();

    $('#statusFilter, #sortOrder').on('change', function() {
        $('#filterForm').submit();
    });

    let searchTimeout;
    $('#searchInput').on('keyup', function() {
        clearTimeout(searchTimeout);

        const term = $(this).val().trim();

        searchTimeout = setTimeout(function() {
            $.ajax({
                url: '/Task/Search',
                type: 'GET',
                data: { term: term },
                success: function(result) {
                    $('#taskTableContainer').html(result);
                },
                error: function() {
                    showAlert('modal', 'Failed to search tasks.', 'error');
                }
            });
        }, 300);
    });

    $(document).on('change', '.toggle-status', function() {
        const checkbox = $(this);
        const taskId = checkbox.data('id');
        const originalState = !checkbox.is(':checked');

        checkbox.prop('disabled', true);

        $.ajax({
            url: '/Task/ToggleStatus',
            type: 'POST',
            data: {
                id: taskId,
                __RequestVerificationToken: token
            },
            success: function(response) {
                if (response.success) {
                    const label = $(`#status-label-${taskId}`);
                    label.text(response.isCompleted ? 'Completed' : 'Pending');

                    const row = checkbox.closest('tr');
                    row.css('backgroundColor', '#d4edda');
                    setTimeout(() => row.css('backgroundColor', ''), 1000);

                    showAlert('toast', 'Task updated successfully.', 'success');
                } else {
                    checkbox.prop('checked', originalState);
                    showAlert('modal', response.message || 'Error updating task status.', 'error');
                }
            },
            error: function() {
                checkbox.prop('checked', originalState);
                showAlert('modal', 'Error updating task status.', 'error');
            },
            complete: function() {
                checkbox.prop('disabled', false);
            }
        });
    });

    $(document).on('click', '.delete-btn', function() {
        const taskId = $(this).data('id');
        const taskTitle = $(this).data('title');

        Swal.fire({
            title: 'Are you sure?',
            html: `You want to delete <strong>${taskTitle}</strong>?<br>This action cannot be undone!`,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#6c757d',
            confirmButtonText: 'Yes, delete it',
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.isConfirmed) {
                deleteTask(taskId);
            }
        });
    });

    function deleteTask(taskId) {
        $.ajax({
        url: '/Task/Delete',
        type: 'POST',
        data: {
            id: taskId,
            __RequestVerificationToken: token
        },
        success: function(response) {
            if (response.success) {
                const row = $(`#task-row-${taskId}`);
                if (row.length) {
                    row.fadeOut(300, function() {
                        $(this).remove();
                    });
                }

                showAlert('toast', response.message || 'Task deleted successfully.', 'success');
            } else {
                showAlert('modal', response.message || 'Delete failed.', 'error');
            }
        },
        error: function() {
            showAlert('modal', 'Error deleting task.', 'error');
        }
    });
    }
});