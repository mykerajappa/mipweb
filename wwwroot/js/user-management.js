$(document).ready(function () {

    $.ajaxSetup({
        headers: {
            'RequestVerificationToken': $('meta[name="csrf-token"]').attr('content')
        }
    });
    
    // Load modal for create user
    $('#createUserBtn').click(function () {
        $.get('/User/Create', function (html) {
            $('#userModalContainer').html(html);
            $('#createUserModal').modal('show');
            $.validator.unobtrusive.parse('#userForm');
        });
    });

    // Edit user
    $(document).on('click', '.edit-user', function () {
        const userId = $(this).closest('tr').data('user-id');
        $.get(`/User/Edit/${userId}`, function (html) {
            $('#userModalContainer').html(html);
            $('#editUserModal').modal('show');
            $.validator.unobtrusive.parse('#userForm');
        });
    });

    // AJAX form submit for create/edit
    $(document).on('submit', '#userForm', function (e) {
        e.preventDefault();
        const form = $(this);
        const spinner = $('#userFormSpinner');
        spinner.removeClass('d-none');

        $.ajax({
            url: form.attr('action') || form.attr('action', window.location.pathname),
            type: 'POST',
            data: form.serialize(),
            success: function (response) {
                if (response.success) {
                    $('.modal').modal('hide');
                    loadUserList();
                } else {
                    $('#userModalContainer').html(response);
                    $.validator.unobtrusive.parse('#userForm');
                }
            },
            error: function () {
                alert('Error occurred while saving user.');
            },
            complete: function () {
                spinner.addClass('d-none');
            }
        });
    });

    // Delete user
    $(document).on('click', '.delete-user', function () {
        const userId = $(this).closest('tr').data('user-id');
        if (confirm('Are you sure you want to delete this user?')) {
            $.post(`/User/Delete/${userId}`, function () {
                loadUserList();
            });
        }
    });

    // Deactivate user
    $(document).on('click', '.deactivate-user', function () {
        const userId = $(this).closest('tr').data('user-id');
        if (confirm('Deactivate this user?')) {
            $.post(`/User/Deactivate/${userId}`, function () {
                loadUserList();
            });
        }
    });

    // Reset password
    $(document).on('click', '.reset-password', function () {
        const userId = $(this).closest('tr').data('user-id');
        if (confirm('Reset password for this user?')) {
            $.post(`/User/ResetPassword/${userId}`, function () {
                alert('New password sent to user\'s WhatsApp number.');
            });
        }
    });

    // Reload user list via AJAX
    function loadUserList() {
        $.get('/User/UserListPartial', function (html) {
            $('#userListContainer').html(html);
        });
    }
});
