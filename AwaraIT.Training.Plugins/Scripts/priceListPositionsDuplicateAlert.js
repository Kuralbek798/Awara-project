function handleSaveError(executionContext) {
    var req = new XMLHttpRequest();
    req.open("POST", Xrm.Page.context.getClientUrl() + "/api/data/v9.0/fnt_priceListPositionsDuplicateInfo", true);
    req.setRequestHeader("OData-MaxVersion", "4.0");
    req.setRequestHeader("OData-Version", "4.0");
    req.setRequestHeader("Accept", "application/json"); //Accept-  json
    req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
    req.setRequestHeader("Prefer", "return=representation");

    req.onreadystatechange = function () {

        if (this.readyState === 4) {

            if (this.status === 200) {
                var result = JSON.parse(this.response);
                var errorMessage = result.ErrorMessage;
                if (errorMessage !== null && errorMessage === "Error Duplicate!") {

                    showInfo(errorMessage)
                }
                console.log("[handleSaveError] errorMessage:", errorMessage);
            } else {
                console.error("[handleSaveError] Request failed with status:", this.status);
                console.error("[handleSaveError] Error response:", this.response);
            }
        }
    };

    console.log("[MyScript] Sending API request...");
    req.send(JSON.stringify());
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



