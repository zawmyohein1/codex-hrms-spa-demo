(function ($) {
    function loadList(container, url) {
        if (!container || !url) {
            return;
        }

        $(container).load(url);
    }

    $(document).on('click', '[data-modal-url]', function (event) {
        event.preventDefault();
        const trigger = $(this);
        const url = trigger.data('modal-url');
        const modalSelector = trigger.data('modal-target') || '#modalForm';
        const modalElement = $(modalSelector);

        if (!url || modalElement.length === 0) {
            return;
        }

        $.get(url, function (html) {
            modalElement.find('.modal-content').html(html);
            const modalInstance = bootstrap.Modal.getOrCreateInstance(modalElement[0]);
            modalInstance.show();
        });
    });

    $(document).on('submit', 'form.ajax-form', function (event) {
        event.preventDefault();
        const form = $(this);
        const modalElement = form.closest('.modal');
        const listContainer = form.data('list-target');
        const listUrl = form.data('list-url');

        $.ajax({
            type: form.attr('method'),
            url: form.attr('action'),
            data: form.serialize(),
            success: function (response, status, xhr) {
                const contentType = xhr.getResponseHeader('content-type') || '';

                if (contentType.includes('application/json')) {
                    if (response && response.success) {
                        if (modalElement.length) {
                            const modalInstance = bootstrap.Modal.getInstance(modalElement[0]);
                            if (modalInstance) {
                                modalInstance.hide();
                            }
                        }

                        loadList(listContainer, listUrl);
                    }
                } else {
                    form.closest('.modal-content').html(response);
                }
            },
            error: function () {
                alert('An unexpected error occurred.');
            }
        });
    });

    window.hrms = {
        loadList: loadList
    };
})(jQuery);
