$.ajaxSetup({
    beforeSend: function (xhr) {
        var storeId = localStorage.getItem('selectedStoreId');
        if (storeId) {
            xhr.setRequestHeader('Selected-Store-Id', storeId);
        }
    }
});


$(document).ready(function () {
    
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
    loadAllStores();
    
    function loadAllStores() {
        $.ajax({
            url: '/Stores/GetAllStores',
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
                if (selectedStoreId) {
                    localStorage.setItem('selectedStoreId', selectedStoreId);
                    document.cookie = "SelectedStoreId=" + encodeURIComponent(selectedStoreId) + "; path=/";
                    storeSelect.val(selectedStoreId);
                } else {
                    localStorage.removeItem('selectedStoreId');
                }
            },
            error: function () {
                alert('Failed to load stores.');
            }
        });
    }

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