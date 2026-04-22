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

    $(document).on('click', '.toggle-status', function() {
        const badge = $(this);
        const taskId = badge.data('id');

        $.ajax({
            url: '/Task/ToggleStatus',
            type: 'POST',
            data: {
                id: taskId,
                __RequestVerificationToken: token
            },
            success: function(response) {
                if (response.success) {
                    // Update text
                    badge.text(response.isCompleted ? 'Completed' : 'Pending');

                    // Update color
                    if (response.isCompleted) {
                        badge.removeClass('bg-secondary-subtle text-secondary')
                            .addClass('bg-success-subtle text-success');
                    } else {
                        badge.removeClass('bg-success-subtle text-success')
                            .addClass('bg-secondary-subtle text-secondary');
                    }

                    showAlert('toast', 'Task status updated.', 'success');
                } else {
                    showAlert('modal', response.message || 'Error updating status.', 'error');
                }
            },
            error: function() {
                showAlert('modal', 'Error updating task status.', 'error');
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