window.hrms = window.hrms || {};

(function (exports) {
    let modalInstance = null;

    function getModalInstance() {
        const modalElement = document.getElementById('spaModal');
        if (!modalElement) {
            return null;
        }

        if (!modalInstance) {
            modalInstance = new bootstrap.Modal(modalElement);
        }

        return modalInstance;
    }

    exports.showModal = function (title, bodyHtml) {
        const modalElement = document.getElementById('spaModal');
        if (!modalElement) {
            return;
        }

        const modalTitle = modalElement.querySelector('.modal-title');
        const modalBody = modalElement.querySelector('.modal-body');

        if (modalTitle) {
            modalTitle.textContent = title || '';
        }

        if (modalBody) {
            modalBody.innerHTML = bodyHtml || '';
        }

        const instance = getModalInstance();
        if (instance) {
            instance.show();
        }
    };

    exports.hideModal = function () {
        const instance = getModalInstance();
        if (instance) {
            instance.hide();
        }
    };

    exports.loadPartial = async function (url) {
        const response = await fetch(url, {
            headers: {
                'X-Requested-With': 'XMLHttpRequest'
            }
        });

        if (!response.ok) {
            throw new Error('Unable to load content.');
        }

        return await response.text();
    };

    exports.submitForm = async function (form) {
        const formData = new FormData(form);
        const response = await fetch(form.action, {
            method: form.method || 'POST',
            body: formData,
            headers: {
                'X-Requested-With': 'XMLHttpRequest'
            }
        });

        if (!response.ok) {
            const errorText = await response.text();
            return { success: false, html: errorText };
        }

        const contentType = response.headers.get('content-type') || '';
        if (contentType.includes('application/json')) {
            return await response.json();
        }

        const html = await response.text();
        return { success: true, html };
    };
})(window.hrms);
