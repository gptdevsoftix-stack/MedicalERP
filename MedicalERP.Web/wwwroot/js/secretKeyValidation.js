async function validateSecretKeyIfNeeded() {
    try {
        //let response = await $.ajax({ url: '/Permission/IsSecretKeyRequired', type: 'GET' });

        //if (response.isSecretKeyRequired) {
        debugger
            $('#secretKeyModal').modal('show');
            //common.FormValidation();
            return new Promise((resolve, reject) => {
                $('#confirmSecretKeyBtn').off('click').on('click', async function () {

                    let employeeIdInput = $('#secretKey'); // Assuming employeeId input field ID
                    let errorMessage = $('#employeeIdError');

                    try {
                        let validateResponse = await $.ajax({
                            url: '/Permission/ValidateSecretKey',
                            type: 'POST',
                            data: { secretKey: $('#secretKey').val() }
                        });
                        debugger
                        if (validateResponse.isValid) {
                            $('#secretKeyModal').modal('hide');
                            resolve(true);
                        } else {
                            //common.dangerToast('Invalid Secret Key');
                            // Display an error message
                            if (!errorMessage.length) {
                                employeeIdInput.after('<div id="employeeIdError" class="text-danger mt-2">Invalid Employee ID</div>');
                            } else {
                                errorMessage.text('Invalid Employee ID');
                            }
                            resolve(false);
                        }
                    } catch (error) {
                        //common.dangerToast(error)
                        resolve(false);
                    }
                });
            });
        //} else {
        //    return true;
        //}
    } catch (error) {
        common.dangerToast(error)
        return false;
    }
}

//function FormValidation() {

//    debugger
//    const Form = document.getElementById('secretKeyForm');

//    if (Form) {
//        const fv = FormValidation.formValidation(Form, {
//            fields: {
//                secretKey: {
//                    validators: {
//                        notEmpty: {
//                            message: 'Please enter your secret key'
//                        }
//                    }
//                }
//            },
//            plugins: {
//                trigger: new FormValidation.plugins.Trigger(),
//                bootstrap5: new FormValidation.plugins.Bootstrap5({
//                    eleValidClass: '',
//                    rowSelector: '.col-12, .col-md-6'
//                }),
//                submitButton: new FormValidation.plugins.SubmitButton(),
//                autoFocus: new FormValidation.plugins.AutoFocus()
//            }
//        });
//    }
//}