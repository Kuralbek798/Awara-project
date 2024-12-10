function handleSaveError(executionContext) {
    var eventArgs = executionContext.getEventArgs();

    var error = eventArgs.getError();
    if (error) {
        var errorMessage = error.message;
        var errorCode = error.errorCode;

        if (errorCode === 1001) {
            showInfo(errorMessage);
            eventArgs.preventDefault(); // Предотвращение сохранения
        }
        console.log("[handleSaveError] errorMessage:", errorMessage);
    } else {
        console.error("[handleSaveError] No error information available.");
    }

}

function showInfo(errorMessage) {
    console.log("[handleSaveError] Function errorMessage called");
    console.log("[handleSaveError] Error message:", errorMessage);
    var alertStrings = { confirmButtonLabel: "OK", text: "Позиция прайс-листа с такими данными уже существует. Пожалуйста, измените входные параметры.", title: "Ошибка" };
    var alertOptions = { height: 200, width: 400 };
    Xrm.Navigation.openAlertDialog(alertStrings, alertOptions).then(
        function () {
            console.log("[handleSaveError] Alert dialog closed");
        },
        function (error) {
            console.error("[handleSaveError] Error showing alert dialog:", error);
        }
    );
}
