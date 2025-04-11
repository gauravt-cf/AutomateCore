class ConfigurationTemplate {
    constructor() {
        this.initialConfig = `
<configuration>
    <configSections>
        <section name="schedulerSettings" type="AutomateCore.Configuration.SchedulerConfigSection, AutomateCore" requirePermission="false" />
    </configSections>
    <schedulerSettings>
        <add scheduleType="{scheduleType}" enabled="{enabled}" hour="{hour}" minute="{minute}" dayOfWeek="{dayOfWeek}" dayOfMonth="{dayOfMonth}" dryRun="{dryRun}" />
    </schedulerSettings>
</configuration>`;
    }

    getTemplate(scheduleType, enabled = "true", hour = "0", minute = "0", dayOfWeek = "", dayOfMonth = "0", dryRun = "false") {
        return this.initialConfig
            .replace(/{scheduleType}/g, scheduleType || "")
            .replace(/{enabled}/g, enabled || "true")
            .replace(/{hour}/g, hour || "0")
            .replace(/{minute}/g, minute || "0")
            .replace(/{dayOfWeek}/g, dayOfWeek || "")
            .replace(/{dayOfMonth}/g, dayOfMonth || "0")
            .replace(/{dryRun}/g, dryRun || "false");
    }
}

$(function () {
    const configurationTemplate = new ConfigurationTemplate();
    const template = configurationTemplate.getTemplate("Daily", "true", "1", "15", "", "0", "true");
    $("#config-preview").text(template);

    $(document).on('click','#generateConfig',function(){
        
    })
});
