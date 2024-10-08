$(function () {
    
    $('#dnnModuleSettings form').ajaxForm({
        success: function () {
            window.location = $('#dnnModuleSettings').attr('data-returnurl');
        },
        beforeSerialize: function () {
        }
    });

    $('#cmdDelete').click(function () {
        var action = $(this).attr('data-action');
        $('#dnnModuleSettings form').ajaxSubmit({
            url: action,
            success: function () {
                window.location = $('#dnnModuleSettings').attr('data-returnurl');
            },
        });
        // return false to prevent normal browser submit and page navigation
        return false;
    });
    
});
