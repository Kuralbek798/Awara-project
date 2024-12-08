function handleSaveError(executionContext) {
    var formContext = executionContext.getFormContext();
    formContext.data.entity.addOnSave(function (context) {
        var eventArgs = context.getEventArgs();
        if (eventArgs.getSaveMode() === 70) { // 70 - код ошибки для InvalidPluginExecutionException
            var errorMessage = eventArgs.getError().message;
            if (errorMessage.includes("Error Duplicate!")) {
                eventArgs.preventDefault();
                var alertStrings = { confirmButtonLabel: "OK", text: "Позиция прайс-листа с такими данными уже существует. Пожалуйста, измените входные параметры.", title: "Ошибка" };
                var alertOptions = { height: 200, width: 400 };
                Xrm.Navigation.openAlertDialog(alertStrings, alertOptions);
            }
        }
    });
}

