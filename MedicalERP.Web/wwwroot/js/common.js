var common = {
    isLoaderVisible: false,
    init() {
        common.dataTable();
        common.attachEventHandlers();
        common.loadAllStores();
        common.selectStore();
        common.handleAjaxError();
        common.handleAjaxSucces();
        common.initializeSelect2();
    },

    attachEventHandlers() {

        $('#storeDropdown').change(common.handleStoreChange);
    },

    showLoader: function () {
        $('#render-body-content').fadeOut('fast', function () {   // Hide main content
            $('#content-loader').fadeIn('fast'); // Show loader

        });
    },
    hideLoader: function () {
        $('#content-loader').fadeOut('fast', function () {
            $('#render-body-content').fadeIn('fast'); // Show main content
        });
    },
    selectStore() {

        var storeId = localStorage.getItem('selectedStoreId');

        if (storeId) { document.cookie = "SelectedStoreId=" + encodeURIComponent(storeId) + "; path=/"; }

        else { document.cookie = "SelectedStoreId=" + encodeURIComponent(0) + "; path=/"; }
    },

    initializeSelect2() {


        var select2 = $('.select2');

        if (select2.length) {
            select2.each(function (index, element) {
                var $element = $(element);
                $element.wrap('<div class="position-relative"></div>').select2({
                    placeholder: 'Select value',
                    //allowClear: true,
                    closeOnSelect: true,
                    dropdownParent: $element.parent()
                });
            });
        }
    },
    applyValidationStyles() {
        // Iterate over each Select2 element to check for validation errors
        $('.select2').each(function () {
            const $select = $(this);
            const $select2Container = $select.next('.select2-container');

            // Check if the select element has the input-validation-error class
            if ($select.hasClass('input-validation-error')) {
                $select2Container.addClass('select2-container--error');
            } else {
                $select2Container.removeClass('select2-container--error');
            }
        });
    },
    reapplyValidation: function (formSelector) {

        $(formSelector).removeData("validator").removeData("unobtrusiveValidation");
        $.validator.unobtrusive.parse(formSelector);

    },
    initializeDatePicker() {

        Datepicker = $('.datepicker');

        if (Datepicker.length) {
            Datepicker.datepicker({
                todayHighlight: true,
                format: 'mm/dd/yyyy',
                autoclose: true
            });
        }
    },
    loadAllStores() {
        $.ajax({
            url: '/Stores/GetUserStores',
            type: 'GET',
            success: function (data) {
                var storedata = data;
                var storeSelect = $('#storeDropdown');
                storeSelect.empty();
                storeSelect.append('<option value="">Select Store</option>');

                $.each(storedata, function (i, store) {
                    storeSelect.append('<option value="' + store.storeId + '">' + store.storeName + '</option>');
                });

                // Retrieve the selected store from localStorage
                var selectedStoreId = localStorage.getItem('selectedStoreId');

                // If the selected store is not in the available stores, remove it
                if (selectedStoreId && !storedata.some(s => s.storeId == selectedStoreId)) {
                    localStorage.removeItem('selectedStoreId');
                    document.cookie = "SelectedStoreId=0; path=/";
                    common.warningToast("Please select another store");
                } else if (selectedStoreId) {
                    storeSelect.val(selectedStoreId);
                }
            }
        });
    },
    ensureStoreSelected(required) {
        var selectedStoreId = localStorage.getItem('selectedStoreId');
        if (required && (!selectedStoreId || selectedStoreId === '0')) {
            alert('Please select a store first.');
            // You can redirect the user or prevent page actions
            return false;
        }
        return true;
    },
    handleStoreChange() {
        var selectedStoreId = $('#storeDropdown').val();
        if (selectedStoreId) {
            localStorage.setItem('selectedStoreId', selectedStoreId);
            document.cookie = "SelectedStoreId=" + encodeURIComponent(selectedStoreId) + "; path=/";
        } else {
            localStorage.removeItem('selectedStoreId');
            document.cookie = "SelectedStoreId=0; path=/";
        }
        location.reload();
    },
    dataTable() {

        var $table = $('.datatable');


        $table.DataTable({
            "initComplete": function () {

            },
            "columnDefs": [
                { "targets": -1, "orderable": false }
            ]
        });
    },
    getNestedProperty(obj, path) {
        return path.split('.').reduce((acc, part) => acc && acc[part], obj);
    },
    initializeDataGrid: function (gridId, dataSource, columns, keyExpr, entity, useAjaxForActions = true) {
        // Add an Actions column to the columns array
        var color = common.getCSSVariableValue('--bs-body-color');

        if (useAjaxForActions) {
            columns.push({
                caption: "Actions",
                width: 220,
                cellTemplate: function (container, options) {
                    //const id = options.data[keyExpr];
                    const id = common.getNestedProperty(options.data, keyExpr);// Use keyExpr as the unique identifier field

                    const endpoint = typeof id === "string"
                        ? '/Permission/RenderActionsDropdowns'  // Endpoint for string (e.g., User IDs, GUIDs)
                        : '/Permission/RenderActionsDropdown';
                    // Perform an AJAX request to fetch the partial view content
                    $.ajax({
                        url: endpoint,  // Replace with your controller and action
                        type: 'GET',
                        data: { id: id, entity: entity },
                        success: function (response) {
                            $(container).append(response);  // Append the partial view content to the cell

                        },
                        error: function () {
                            $(container).append('<div class="text-end pe-5">Error loading actions</div>');
                        }
                    });
                },
                alignment: "center",
                allowSorting: false,
                allowFiltering: false
            });
        }
        // Initialize the grid with given columns and configuration
        $("#" + gridId).dxDataGrid({
            dataSource: dataSource,
            keyExpr: keyExpr,  // Set keyExpr dynamically
            columns: columns,
            showBorders: false,
            showColumnLines: false,
            showRowLines: false,
            allowColumnResizing: false,
            //columnResizingMode: 'widget',
            columnAutoWidth: true,
            //columnMinWidth: 150,
            //allowColumnResizing: true,
            //columnResizingMode: 'widget',
            //allowColumnReordering: true,
            //width: "100border-bottom: 1px solid green;
            //columnResizingMode: "widget", 
            onRowClick: function (e) {
                var $rowElement = $(e.rowElement);
                $rowElement.toggleClass('highlighted');
            },
            rowAlternationEnabled: false, // Optional: for alternating row background
            hoverStateEnabled: false,  
            onRowPrepared: function (e) {
                // Apply the color to the row element (for the entire row)
                e.rowElement.css("color", color);
                e.rowElement.css("border-bottom", "1px solid #666ee863");
            },
            onCellPrepared: function (e) {
                // Apply the color dynamically to each cell
                if (e.rowType === "data" && e.columnIndex === 0) { // Target only the first cell in each data row
                    e.cellElement.closest(".dx-row").css("border-bottom", "1px solid #666ee863");
                }
            },

            // Paging
            paging: {
                pageSize: 50
            },
            pager: {
                showPageSizeSelector: true,
                allowedPageSizes: [10, 20, 50, 100],
                showInfo: true,
                showNavigationButtons: true
            },

            // Search and filtering across all columns
            searchPanel: {
                visible: true,
                width: 240,
                placeholder: "Search..."
            },
            filterRow: {
                visible: true,
                applyFilter: "auto"
            },
            headerFilter: {
                visible: true
            },

            // Remove selection checkbox column
            selection: {
                mode: "none"
            },

        });

        setTimeout(() => {

            const gridInstance = $("#dataGrid").dxDataGrid("instance");
            if (gridInstance) {
                gridInstance.repaint();
            } // Repaint grid after page change with delay
        }, 500);


    },

    getCSSVariableValue(variable) {

        return getComputedStyle(document.documentElement).getPropertyValue(variable).trim();
    },

    toast(text, type) {

        if (type == 0) { common.successToast(text) }
        else if (type == 1) { common.warningToast(text) }

    },
    infoToast(text) {
        var color = common.getCSSVariableValue('--bs-body-color');
        var bgColor = common.getCSSVariableValue('--bs-body-bg');
        var successColor = common.getCSSVariableValue('--bs-info');
        var colorScheme = common.getCSSVariableValue('color-scheme');
        var iziToastTheme = colorScheme === 'dark' ? 'dark' : 'light';

        iziToast.info({
            message: text,
            position: 'topRight',
            progressBar: true,
            backgroundColor: bgColor,
            theme: iziToastTheme,
            transitionIn: 'fadeInLeft',
            transitionOut: 'fadeOutRight',
            onOpening: function (instance, toast) { toast.style.borderLeft = `10px solid ${successColor}`; }
        });
    },
    successToast(text) {
        var color = common.getCSSVariableValue('--bs-body-color');
        var bgColor = common.getCSSVariableValue('--bs-body-bg');
        var successColor = common.getCSSVariableValue('--bs-success');
        var colorScheme = common.getCSSVariableValue('color-scheme');
        var iziToastTheme = colorScheme === 'dark' ? 'dark' : 'light';

        iziToast.success({
            message: text,
            position: 'topRight',
            progressBar: true,
            backgroundColor: bgColor,
            theme: iziToastTheme,
            transitionIn: 'fadeInLeft',
            transitionOut: 'fadeOutRight',
            onOpening: function (instance, toast) { toast.style.borderLeft = `10px solid ${successColor}`; }
        });
    },

    warningToast(text) {

        var bgColor = common.getCSSVariableValue('--bs-body-bg');
        var warningColor = common.getCSSVariableValue('--bs-warning');
        var colorScheme = common.getCSSVariableValue('color-scheme');
        var iziToastTheme = colorScheme === 'dark' ? 'dark' : 'light';

        iziToast.warning({
            message: text,
            position: 'topRight',
            backgroundColor: bgColor,
            theme: iziToastTheme,
            progressBar: true,
            transitionIn: 'fadeInLeft',
            transitionOut: 'fadeOutRight',
            onOpening: function (instance, toast) { toast.style.borderLeft = `10px solid ${warningColor}`; }
        });
    },

    dangerToast(text) {

        var bgColor = common.getCSSVariableValue('--bs-body-bg');
        var dangerColor = common.getCSSVariableValue('--bs-danger');
        var colorScheme = common.getCSSVariableValue('color-scheme');
        var iziToastTheme = colorScheme === 'dark' ? 'dark' : 'light';

        iziToast.error({
            message: text,
            position: 'topRight',
            backgroundColor: bgColor,
            theme: iziToastTheme,
            progressBar: true,
            transitionIn: 'fadeInLeft',
            transitionOut: 'fadeOutRight',
            onOpening: function (instance, toast) { toast.style.borderLeft = `10px solid ${dangerColor}`; }
        });
    },
    handleAjaxError() {

        $(document).ajaxError(function (event, jqXHR, ajaxSettings, thrownError) {
            var message;
            if (jqXHR.status === 0) {
                message = 'Not connected. Please verify your network connection.';
            } else if (jqXHR.status == 404) {
                message = 'Requested page not found [404].';
            } else if (jqXHR.status == 500) {
                try {
                    const errorResponse = JSON.parse(jqXHR.responseText);
                    message = errorResponse;
                } catch (e) {
                    message = jqXHR.responseText;
                }
            } else if (thrownError === 'parsererror') {
                message = 'Requested JSON parse failed.';
            } else if (thrownError === 'timeout') {
                message = 'Time out error.';
            } else if (thrownError === 'abort') {
                message = 'Ajax request aborted.';
            } else {
                message =  jqXHR.responseText;
            }
            //common.dangerToast(message);
            alert(message);

        });
    },
    handleAjaxSucces() {
        $(document).ajaxComplete(function (event, jqxhr, settings) {
            // Code to execute after every AJAX request
            common.initializeSelect2();
            common.initializeDatePicker();

            $.validator.unobtrusive.parse(document); // Apply to the entire document

            // Attach change event to all Select2 elements to trigger validation on change
            $('select.select2-hidden-accessible').on('change', function () {
                $(this).valid(); // Trigger validation for the Select2 field
            });
        });
    },
    validateEmployeeId() {
        return new Promise((resolve, reject) => {
            // Show SweetAlert2 input dialog for employee ID
            Swal.fire({
                title: 'Enter Employee ID',
                input: 'text',
                customClass: {
                    input: 'masked-input',
                },
                inputLabel: 'Your Employee ID',
                inputPlaceholder: 'Type your Employee ID here...',
                showCancelButton: true,
                confirmButtonText: 'Submit',
                cancelButtonText: 'Cancel',
                customClass: {
                    confirmButton: 'btn btn-primary me-3',
                    cancelButton: 'btn btn-label-secondary'
                },
                inputAttributes: {
                    autocomplete: 'new-password', // Modern browsers handle "new-password" better than "off"
                    autocorrect: 'off',          // Disable autocorrect
                    autocapitalize: 'none' 
                },
                preConfirm: (employeeId) => {
                    if (!employeeId) {
                        Swal.showValidationMessage('Please enter your Employee ID');
                    }
                    return employeeId;
                }
            }).then((result) => {
                if (result.isConfirmed) {
                    // Validate the Employee ID via an AJAX request
                    $.ajax({
                        type: "POST",
                        url: '/Permission/ValidateSecretKey', // Adjust the URL if needed
                        data: { secretKey: result.value },
                        success: function (validationResponse) {
                            if (validationResponse.isValid) {
                                resolve(result.value); // Resolve the promise if valid
                            } else {
                                Swal.fire({
                                    icon: 'error',
                                    title: 'Invalid Employee ID',
                                    text: 'The Employee ID you entered is incorrect.',
                                    customClass: {
                                        confirmButton: 'btn btn-danger'
                                    }
                                });
                                common.hideLoader();
                                //common.showErrorDialog('Invalid Employee ID', 'The Employee ID you entered is incorrect.');
                                reject('Invalid Employee ID');
                            }
                        },
                        error: function () {
                            Swal.fire({
                                icon: 'error',
                                title: 'Error!',
                                text: 'An error occurred while validating the Employee ID.',
                                customClass: {
                                    confirmButton: 'btn btn-danger'
                                }
                            });
                            //common.showErrorDialog('Error!', 'An error occurred while validating the Employee ID.');
                            reject('Error during validation');
                        }
                    });
                } else {
                    reject('Cancelled');
                }
            });
        });
    },
    showSuccessDialog(title, text) {
        return Swal.fire({
            icon: 'success',
            title: title,
            text: text,
            customClass: {
                confirmButton: 'btn btn-success'
            }
        });
    },

    showErrorDialog(title, text) {
        return Swal.fire({
            icon: 'error',
            title: title,
            text: text,
            customClass: {
                confirmButton: 'btn btn-danger'
            }
        });
    },
    showWarningDialog(title, text) {
        return Swal.fire({
            icon: 'warning',
            title: title,
            text: text,
            customClass: {
                confirmButton: 'btn btn-warning'
            }
        });
    },

    // Info Dialog
    showInfoDialog(title, text) {
        return Swal.fire({
            icon: 'info',
            title: title,
            text: text,
            customClass: {
                confirmButton: 'btn btn-info'
            }
        });
    },
    confirmDeletion() {
        return Swal.fire({
            title: 'Are you sure?',
            text: "You won't be able to revert this!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Yes, delete it!',
            customClass: {
                confirmButton: 'btn btn-primary me-3',
                cancelButton: 'btn btn-label-secondary'
            },
            buttonsStyling: false
        });
    },
    loadForm(actionUrl, data = {}, options = {}, callback = null) {
        debugger
        $.get(actionUrl, data, function (response) {
            const modalId = options.modalId || '#Modal'; // Default modal ID
            const title = options.title || 'Form'; // Default modal title
            const isView = options.isView || false; // Whether it's in view-only mode

            // Load response into modal body and set modal title
            $(`${modalId} .modal-body`).html(response);
            $(`${modalId} .modal-title`).text(title);

            if (isView) {
                // Make all inputs, selects, and textareas readonly
                $(`${modalId} input`).prop('readonly', true);
                $(`${modalId} select`).prop('disabled', true);
                $(`${modalId} textarea`).prop('readonly', true);

                // Hide the submit button (assuming it's type="submit")
                $(`${modalId} button[type="submit"]`).remove();

                $(`${modalId} button`).not('[data-bs-dismiss="modal"]').prop('disabled', true);
                $(`${modalId} button:not(.btn-close)[data-bs-dismiss="modal"]`).text('Close');
            }


            // Show the modal
            $(modalId).modal('show');
            debugger
            // If a callback function is provided, execute it after the modal is shown
            if (typeof callback === 'function') {
                callback();
            }
        });
    },

    submitForm(formSelector, actionUrl) {
        debugger
        $(document).on('submit', formSelector, function (e) {
         
            e.preventDefault();
            var form = $(this);
            $('#Modal').modal('hide');

            // Validate employee ID before making the ajax request  
            common.validateEmployeeId()
                .then((employeeId) => {
                    common.showLoader();
                    $.ajax({
                        url: actionUrl || form.attr('action'),
                        type: form.attr('method'),
                        data: form.serialize(),
                        success: function (result) {
                         
                            debugger

                            if (result.success == undefined) {
                                common.hideLoader();
                                common.warningToast("Validation error");
                                $('#Modal .modal-body').html(result);
                                $('#Modal').modal('show');
                            }
                            else {

                                if (result.success) {
                                    debugger
                                    common.successToast('Success!', result.message)
                                    $('#Modal').modal('hide');
                                    setTimeout(function () {
                                        location.reload();
                                    }, 1000);

                                } else if (!result.success) {
                                    common.hideLoader();
                                    $('#Modal').modal('show');
                                    common.warningToast(result.message);
                                }


                            }

                        }
                    });

                })
                .catch((error) => {
                    console.error(error); // For logging purposes
                });
        });

    },
    handleDelete(url, id, successMessage) {

        return common.validateEmployeeId() // Validate employee ID before proceeding
            .then((employeeId) => {
                return common.confirmDeletion(); // Confirm the deletion
            })
            .then((deleteResult) => {
                if (deleteResult.isConfirmed) {
                    return $.ajax({
                        type: "POST",
                        url: url, // Dynamic URL for the delete request
                        data: { id: id }, // Pass the ID dynamically
                        success: function (response) {
                            if (response.success) {
                                return common.showSuccessDialog('Deleted!', response.message)
                                    .then(() => location.reload()); // Reload the page after deletion
                            } else {
                                return common.showErrorDialog('Error!', response.message);
                            }
                        },
                        error: function () {
                            return common.showErrorDialog('Error!', 'An error occurred while deleting.');
                        }
                    });
                }
            })
            .catch((error) => {
                console.error(error);
            });
    },




};

//function loadNotifications() {
//    fetch('/Notification/GetUnread')
//        .then(response => response.json())
//        .then(data => {
//            const count = data.length;
//            $('#notification-count').text(count);

//            const $list = $('#notification-list');
//            $list.empty();

//            if (count === 0) {
//                $list.append('<li class="text-center py-3 text-muted">No new notifications.</li>');
//            } else {
//                data.forEach(n => {
//                    const item = `
//                        <li class="list-group-item list-group-item-action dropdown-notifications-item">
//                            <div class="d-flex">
//                                <div class="flex-shrink-0 me-3">
//                                    <div class="avatar">
//                                        <span class="avatar-initial rounded-circle bg-label-success">
//                                            ${getNotificationIcon(n.eventType)}
//                                        </span>
//                                    </div>
//                                </div>
//                                <div class="flex-grow-1">
//                                    <h6 class="mb-1">${n.eventType}</h6>
//                                    <p class="mb-0">${n.message}</p>
//                                    <small class="text-muted">${new Date(n.createdAt).toLocaleString()}</small>
//                                </div>
//                                <div class="flex-shrink-0 dropdown-notifications-actions">
//                                    <a href="javascript:void(0)" class="dropdown-notifications-archive" onclick="markAsRead(${n.notificationId})">
//                                        <i class="ti ti-x text-danger fs-5"></i>
//                                    </a>
//                                </div>
//                            </div>
//                        </li>`;

//                    $list.append(item);
//                });
//            }
//        });
//}


//async function loadNotifications() {
//    const response = await fetch('/Notification/GetUnread');
//    const notifications = await response.json();

//    const list = document.getElementById('notification-list');
//    list.innerHTML = '';

//    if (notifications.length === 0) {
//        list.innerHTML = '<li class="text-center py-3 text-muted">No new notifications.</li>';
//    } else {
//        notifications.forEach(notification => {
//            const storeLink = notification.storeId
//                ? `<br><a href="/SaleOrder/ViewOrder?storeId=${notification.storeId}" class="text-primary small">View Store Sale Orders</a>`
//                : '';

//            const item = `
//                <li class="list-group-item list-group-item-action">
//                    <div class="d-flex justify-content-between align-items-center">
//                        <span>${notification.message}</span>
//                        <button class="btn btn-sm " onclick="markAsRead(${notification.notificationId})">
//                            X
//                        </button>
//                    </div>
//                    ${storeLink}
//                </li>`;

//            list.innerHTML += item;
//        });
//    }

//    // Update the notification count badge
//    document.getElementById('notification-count').innerText = notifications.length;
//}







//function markAsRead(notificationId) {
//    fetch('/Notification/MarkAsRead?id=' + notificationId, { method: 'POST' })
//        .then(() => loadNotifications());
//}

//function markAllAsRead() {
//    fetch('/Notification/MarkAllAsRead', { method: 'POST' })
//        .then(() => loadNotifications());
//}

//$(document).ready(function () {
//    loadNotifications();
//    setInterval(loadNotifications, 30000); // Auto-refresh
//});

$(document).ready(function () {
    common.init();

    $.validator.setDefaults({
        highlight: function (element) {
            $(element).closest('.form-control').addClass('is-invalid'); // Adds red border to invalid fields
            if ($(element).hasClass('select2-hidden-accessible')) {
                // Apply styling to Select2 wrapper for select fields
                $(element).next('.select2-container').find('.select2-selection').addClass('is-invalid');
            }
        },
        unhighlight: function (element) {
            $(element).closest('.form-control').removeClass('is-invalid'); // Removes red border for valid fields
            if ($(element).hasClass('select2-hidden-accessible')) {
                // Remove styling from Select2 wrapper
                $(element).next('.select2-container').find('.select2-selection').removeClass('is-invalid');
            }
        }
    });
});
$(window).on('load', function () {
    // Hide the content loader and show the main content when the page is ready
    common.hideLoader();
});





function getNotificationIcon(eventType) {
    switch (eventType) {
        case 'SaleOrder':
            return '<i class="ti ti-currency-dollar text-success fs-4"></i>'; // Sales Icon
        case 'PurchaseOrder':
            return '<i class="ti ti-shopping-cart text-warning fs-4"></i>'; // Purchase Icon
        case 'RepairOrder':
            return '<i class="ti ti-tools text-primary fs-4"></i>'; // Repair Icon
        case 'ReturnToStock':
            return '<i class="ti ti-archive text-info fs-4"></i>'; // Return Icon
        case 'VendorReturn':
            return '<i class="ti ti-truck-return text-danger fs-4"></i>'; // Vendor Return Icon
        default:
            return '<i class="ti ti-bell text-secondary fs-4"></i>'; // Default Notification Icon
    }
}


