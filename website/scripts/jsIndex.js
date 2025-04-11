var githubSerice = new GitHubSnippetDataService();
$(function () {
    githubSerice.getSnippetByKey("basicUsage").then((response) => {
        $("#basicUsageExample").html(response.code);
    }).catch((error) => {
        showErrorBanner('Could not load snippets from GitHub. Please check your internet or try again later.');
        return null;
    });
    githubSerice.getSnippetByKey("xmlConfiguration").then((response) => {
        $("#xmlConfigurationExample").text(response.code);
    }).catch((error) => {
        showErrorBanner('Could not load snippets from GitHub. Please check your internet or try again later.');
        return null;
    })
    githubSerice.getSnippetByKey("basicUsageWithLifeCycleCallbacks").then((response) => {
        $("#LifecycleCallbacksExample").html(response.code);
    }).catch((error) => {
        showErrorBanner('Could not load snippets from GitHub. Please check your internet or try again later.');
        return null;
    });
    loadXmlConfigurationOptionTable();
});
function loadXmlConfigurationOptionTable() {
    githubSerice.getSnippetByKey("xmlConfigurationAttributes").then((response) => {
        response.attributes.forEach(attr => {
            const row = document.createElement("tr");
            row.classList.add("border-t");

            row.innerHTML = `
              <td class="px-4 py-2 font-medium text-gray-700">${attr.name}</td>
              <td class="px-4 py-2 text-gray-600">${attr.description}</td>
            `;
            $("#xmlConfigTableBody").append(row);
        });
    }).catch((error) => {
        showErrorBanner('Could not load snippets from GitHub. Please check your internet or try again later.');
        return null;
    });
}