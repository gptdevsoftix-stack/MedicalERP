$.ajaxSetup({
    beforeSend: function (xhr) {
        var storeId = localStorage.getItem('selectedStoreId');
        if (storeId) {
            xhr.setRequestHeader('Selected-Store-Id', storeId);
        }
    }
});


$(document).ready(function () {
    var companySelect = $('#companyDropdown');
    var selectedCompanyId = companySelect.data('selected-company-id') || getQueryParameter('companyContextId') || getCookie('SelectedCompanyId');

    if (selectedCompanyId) {
        document.cookie = 'SelectedCompanyId=' + encodeURIComponent(selectedCompanyId) + '; path=/; SameSite=Lax';
    }

    if (companySelect.length) {
        loadAllCompanies();
    }

        var storeId = localStorage.getItem('selectedStoreId');
    if (storeId) {
        document.cookie = "SelectedStoreId=" + encodeURIComponent(storeId) + "; path=/";
        console.log(getCookie('SelectedStoreId'));
    }
    else { document.cookie = "SelectedStoreId=" + encodeURIComponent(0) + "; path=/"; }
    function getCookie(name) {
        var value = "; " + document.cookie;
        var parts = value.split("; " + name + "=");
        if (parts.length === 2) return parts.pop().split(";").shift();
    }
    if (!companySelect.length || selectedCompanyId) {
        loadAllStores();
    } else {
        $('#storeDropdown').prop('disabled', true);
    }

    function getQueryParameter(name) {
        return new URLSearchParams(window.location.search).get(name);
    }

    function loadAllCompanies() {
        $.get('/Companies/GetAllCompanies', function (companies) {
            companySelect.empty().append('<option value="">Select Company</option>');
            $.each(companies, function (_, company) {
                companySelect.append($('<option>', {
                    value: company.companyId,
                    text: company.companyName + ' (' + company.companyCode + ')'
                }));
            });
            companySelect.val(selectedCompanyId || '');
        }).fail(function () {
            companySelect.empty().append('<option value="">Unable to load companies</option>');
        });
    }
    
    function loadAllStores() {
        $.ajax({
            url: '/Stores/GetAllStores',
            data: selectedCompanyId ? { companyContextId: selectedCompanyId } : {},
            type: 'GET',
            success: function (data) {
                var storedata = data;
                var storeSelect = $('#storeDropdown');
                storeSelect.empty();
                storeSelect.append('<option value="">Select Store</option>');
                $.each(storedata, function (i, store) {
                    storeSelect.append('<option value="' + store.storeId + '">' + store.storeName + '</option>');
                });
                // Prefer the server-selected store after login so stale browser
                // state from another user cannot override the login default.
                var serverSelectedStore = storedata.find(function (store) { return store.isSelected; });
                var selectedStoreId = serverSelectedStore
                    ? serverSelectedStore.storeId
                    : localStorage.getItem('selectedStoreId');
                var selectedStoreExists = selectedStoreId && storedata.some(function (store) {
                    return store.storeId === selectedStoreId;
                });
                if (selectedStoreExists) {
                    localStorage.setItem('selectedStoreId', selectedStoreId);
                    document.cookie = "SelectedStoreId=" + encodeURIComponent(selectedStoreId) + "; path=/";
                    storeSelect.val(selectedStoreId);
                } else {
                    localStorage.removeItem('selectedStoreId');
                    document.cookie = 'SelectedStoreId=; path=/; max-age=0';
                }
            },
            error: function () {
                alert('Failed to load stores.');
            }
        });
    }

    companySelect.change(function () {
        var companyId = $(this).val();
        localStorage.removeItem('selectedStoreId');
        document.cookie = 'SelectedStoreId=; path=/; max-age=0';

        var url = new URL(window.location.href);
        if (companyId) {
            document.cookie = 'SelectedCompanyId=' + encodeURIComponent(companyId) + '; path=/; SameSite=Lax';
            url.searchParams.set('companyContextId', companyId);
        } else {
            document.cookie = 'SelectedCompanyId=; path=/; max-age=0';
            url.searchParams.delete('companyContextId');
        }
        url.searchParams.delete('storeId');
        url.searchParams.delete('storeContextId');
        window.location.assign(url.toString());
    });

    $('#storeDropdown').change(function () {
        var selectedStoreId = $(this).val();
        if (selectedStoreId) {
            // Handle store selection
            //console.log('Selected Store ID: ' + selectedStoreId);

            // Store the selected store in localStorage
            localStorage.setItem('selectedStoreId', selectedStoreId);
            document.cookie = "SelectedStoreId=" + encodeURIComponent(selectedStoreId) + "; path=/";
        } else {
            // Remove the selected store from localStorage if none is selected
            localStorage.removeItem('selectedStoreId');
            document.cookie = "SelectedStoreId=" + encodeURIComponent(0) + "; path=/";
        }
        location.reload();
    });
});
