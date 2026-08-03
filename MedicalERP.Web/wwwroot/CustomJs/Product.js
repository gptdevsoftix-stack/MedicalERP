$(function () {
    function loadProductForm(actionUrl, data = {}) {
        $.get(actionUrl, data, function (response) {
            $('#productModal .modal-body').html(response);
            $('#productModal').modal('show');

            var storeId = localStorage.getItem('selectedStoreId');
            var storeIdInt = parseInt(storeId, 10);
            $('#storeId').val(storeIdInt);

            // Get existing values from the form
            var selectedCategoryId = $('#CategoryId').data('selected-category');
            var selectedSubCategoryId = $('#SubCategoryId').data('selected-sub-category');

            loadCategories("Product", selectedCategoryId);
            if (selectedCategoryId) {
                loadSubCategories(selectedCategoryId, selectedSubCategoryId);
                loadCategoryType(selectedCategoryId);
            }
        });
    }

    function loadCategories(type, selectedCategoryId = null) {
        $.ajax({
            url: '@Url.Action("GetCategories", "Product")',
            type: 'GET',
            data: { type: type },
            success: function (data) {
                var categorySelect = $('#CategoryId');
                categorySelect.empty();
                categorySelect.append('<option value="">Select Category</option>');
                $.each(data, function (i, category) {
                    categorySelect.append('<option value="' + category.categoryId + '">' + category.categoryName + '</option>');
                });
                if (selectedCategoryId) {
                    categorySelect.val(selectedCategoryId);
                    loadSubCategories(selectedCategoryId, $('#SubCategoryId').data('selected-sub-category'));
                }
            }
        });
    }

    function loadSubCategories(categoryId, selectedSubCategoryId = null) {
        $.ajax({
            url: '@Url.Action("GetSubCategoriesByCategoryId", "Product")',
            type: 'GET',
            data: { categoryId: categoryId },
            success: function (data) {
                var subCategorySelect = $('#SubCategoryId');
                subCategorySelect.empty();
                subCategorySelect.append('<option value="">Select Sub-Category</option>');
                $.each(data, function (i, subCategory) {
                    subCategorySelect.append('<option value="' + subCategory.subCategoryId + '">' + subCategory.subCategoryName + '</option>');
                });
                if (selectedSubCategoryId) {
                    subCategorySelect.val(selectedSubCategoryId);
                    loadAttributes(selectedSubCategoryId); // Load attributes for the selected subcategory
                }

                subCategorySelect.change(function () {
                    var selectedSubCategoryId = $(this).val();
                    loadAttributes(selectedSubCategoryId); // Load attributes when subcategory changes
                });
            }
        });
    }

    function loadAttributes(subCategoryId) {
        $.ajax({
            url: '@Url.Action("GetAttributesBySubCategoryId", "Product")',
            type: 'GET',
            data: { subCategoryId: subCategoryId },
            success: function (attributes) {
                debugger
                var attributesContainer = $('#subcategory-attributes');
                attributesContainer.empty();

                $.each(attributes, function (index, attribute) {
                    var attributeHtml = '';

                    if (attribute.attributeType === 'Dropdown') {
                        attributeHtml = '<div class="mb-3">' +
                            '<label class="form-label">' + attribute.attributeName + '</label>' +
                            '<select class="form-select" name="SubCategoryAttributes[' + index + '].SelectedValues">' +
                            '<option value="">Select</option>';

                        $.each(attribute.attributeValues, function (i, value) {
                            attributeHtml += '<option value="' + value.value + '">' + value.value + '</option>';
                        });

                        attributeHtml += '</select>' +
                            '<input type="hidden" name="SubCategoryAttributes[' + index + '].SubCategoryAttributeId" value="' + attribute.subCategoryAttributeId + '">' +
                            '<input type="hidden" name="SubCategoryAttributes[' + index + '].AttributeName" value="' + attribute.attributeName + '">' +
                            '<input type="hidden" name="SubCategoryAttributes[' + index + '].AttributeType" value="' + attribute.attributeType + '">' +
                            '</div>';
                    } else if (attribute.attributeType === 'Checkbox') {
                        attributeHtml = '<div class="mb-3">' +
                            '<label class="form-label">' + attribute.attributeName + '</label>' +
                            '<div>';

                        $.each(attribute.attributeValues, function (i, value) {
                            attributeHtml += '<div class="form-check">' +
                                '<input class="form-check-input" type="checkbox" name="SubCategoryAttributes[' + index + '].SelectedValues" value="' + value.value + '">' +
                                '<label class="form-check-label">' + value.value + '</label>' +
                                '</div>';
                        });

                        attributeHtml += '</div>' +
                            '<input type="hidden" name="SubCategoryAttributes[' + index + '].SubCategoryAttributeId" value="' + attribute.subCategoryAttributeId + '">' +
                            '<input type="hidden" name="SubCategoryAttributes[' + index + '].AttributeName" value="' + attribute.attributeName + '">' +
                            '<input type="hidden" name="SubCategoryAttributes[' + index + '].AttributeType" value="' + attribute.attributeType + '">' +
                            '</div>';
                    } else {
                        // Handle other attribute types like text, number, etc.
                        attributeHtml = '<div class="mb-3">' +
                            '<label class="form-label">' + attribute.attributeName + '</label>' +
                            '<input type="text" class="form-control" name="SubCategoryAttributes[' + index + '].SelectedValues" required="' + attribute.isRequired + '">' +
                            '<input type="hidden" name="SubCategoryAttributes[' + index + '].SubCategoryAttributeId" value="' + attribute.subCategoryAttributeId + '">' +
                            '<input type="hidden" name="SubCategoryAttributes[' + index + '].AttributeName" value="' + attribute.attributeName + '">' +
                            '<input type="hidden" name="SubCategoryAttributes[' + index + '].AttributeType" value="' + attribute.attributeType + '">' +
                            '</div>';
                    }

                    attributesContainer.append(attributeHtml);
                });
            }
        });
    }

    function loadCategoryType(categoryId) {
        $.get('@Url.Action("GetCategoryType", "Product")', { categoryId: categoryId }, function (response) {
            $('#CategoryType').val(response.type);
        });
    }

    $('#createProductBtn').click(function () {
        loadProductForm('/Product/ProductForm');
    });

    $('.edit-product-btn').click(function () {
        var productId = $(this).data('id');
        loadProductForm('/Product/ProductForm', { id: productId });
    });

    $(document).on('change', '#CategoryId', function () {
        var categoryId = $(this).val();
        if (categoryId) {
            loadCategoryType(categoryId);
            loadSubCategories(categoryId);
        } else {
            $('#CategoryType').val('');
            $('#SubCategoryId').empty().append('<option value="">Select Sub-Category</option>');
            $('#subcategory-attributes').empty(); // Clear attributes if no category selected
        }
    });

    $(document).on('submit', '#productForm', function (e) {

        debugger
        e.preventDefault();
        var form = $(this);
        var formData = new FormData(this);

        $.ajax({
            url: form.attr('action'),
            url: form.attr('action'),
            type: form.attr('method'),
            data: formData,
            processData: false,
            contentType: false,
            success: function (result) {
                if (result.success) {
                    $('#productModal').modal('hide');
                    location.reload();
                } else {
                    $('.modal-body').html(result);
                    var selectedCategoryId = $('#CategoryId').val();
                    var selectedSubCategoryId = $('#SubCategoryId').val();
                    loadCategories("Product", selectedCategoryId);
                    if (selectedCategoryId) {
                        loadSubCategories(selectedCategoryId, selectedSubCategoryId);
                    }
                }
            }
        });
    });
});