class ConfigurationTemplate {
    constructor() {
        this.initialConfig = `<?xml version="1.0" encoding="utf-8" ?>
            <configuration>
                <configSections>
                    <section name="schedulerSettings" type="AutomateCore.Configuration.SchedulerConfigSection, AutomateCore" requirePermission="false" />
                </configSections>   
                <schedulerSettings>
                        <add 
                        scheduleType="{scheduleType}" 
                        enabled="{enabled}" 
                        hour="{hour}" 
                        minute="{minute}" 
                        dayOfWeek="{dayOfWeek}" 
                        dayOfMonth="{dayOfMonth}" 
                        dryRun="{dryRun}" />
                </schedulerSettings>
                    
                <appSettings>
                        <add key="Logging:LogLevel:Default" value="Information" />
                </appSettings>
                    
                <runtime>
                        <gcServer enabled="true" />
                        <gcConcurrent enabled="true" />
                </runtime>
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
const configurationTemplate = new ConfigurationTemplate();
var githubSerice = new GitHubSnippetDataService("web-store", "schdule-type.json");
class configBuilder {
    constructor() {
        this.$schduleTypeDropdown = $("#scheduleType");
        this.$schduleTypeDropdown.empty();
    }
    bindScheduleTypes(scheduleTypes) {
        if (Array.isArray(scheduleTypes) && scheduleTypes.length > 0) {
            scheduleTypes.forEach(item => {
                this.$schduleTypeDropdown.append(
                    $('<option>', {
                        value: item.value,
                        text: item.label,
                        'data-properties': JSON.stringify(item.properties)
                    })
                );
            });
        } else {
            let option = `<option value="">-- Select Schedule Type --</option>`;
            this.$schduleTypeDropdown.html(option);
        }
    }
    showFields(schduleType, properties) {
        schduleType = $.trim(schduleType.toString()).toLowerCase();

        $("#div_enabled, #div_dryRun, #common-fields-div").removeClass('hide').addClass('show');
        $("#div_hour, #div_minute,#div_dayOfMonth,#div_dayOfWeek").addClass('hide').removeClass('show');

        const props = (properties || []).map(p => $.trim(p.toString().toLowerCase()));
        console.log(props);
        if (props.includes("hour")) {
            $("#div_hour").removeClass('hide').addClass('show');
        }
        if (props.includes("minute")) {
            $("#div_minute").removeClass('hide').addClass('show');
        }
        if (props.includes("dayofmonth")) {
            $("#div_dayOfMonth").removeClass('hide').addClass('show');
        }
        if (props.includes("dayofweek")) {
            $("#div_dayOfWeek").removeClass('hide').addClass('show');
        }
    }
}
var _configBuilder = new configBuilder();
function showErrorBanner2(message) {
    const banner = $("#error-banner");
    const text = $("#error-banner-text");
    text.text(message);
    banner.removeClass("hidden");

    // Optional: Auto-hide after 5 seconds
    setTimeout(() => {
        banner.addClass("hidden");
        text.text("");
    }, 5000);
}

$(function () {
    $('#scheduleType').select2({
        placeholder: '-- Select Schedule Type --',
        width: '100%',
        theme: 'classic'
    });
    $('#drpDryRun,#dayOfWeek,#enabled').select2({
        placeholder: '-- Select --',
        width: '100%',
        theme: 'classic'
    });
    githubSerice.getAllScheduleType().then((response) => {
        _configBuilder.bindScheduleTypes(response);
    }).catch((error) => {
        showErrorBanner('Could not load snippets from GitHub. Please check your internet or try again later.');
        return null;
    });

    $('#scheduleType').on('change', function (event) {
        const selected = $(this).find(':selected');
        const value = selected.val();
        const properties = JSON.parse(selected.attr('data-properties') || '[]');
        _configBuilder.showFields(value, properties);
    });
    $('#generateConfig').on('click', function (event) {
        event.preventDefault();
        $(".text-red-500").addClass("hidden").text("");
        let $scheduleTypeDropdown = $('#scheduleType');
        const selectedOption = $scheduleTypeDropdown.find(':selected');
        const value = selectedOption.val();

        if (!value) {
            showErrorBanner2("Please select a Schedule Type.");
            return;
        }

        const properties = JSON.parse(selectedOption.attr('data-properties') || '[]');

        let isEnabledVisible = $("#div_enabled:visible").length > 0;
        let isDryRunVisible = $("#div_dryRun:visible").length > 0;
        let isDivHourVisible = $("#div_hour:visible").length > 0;
        let isDivMinuteVisible = $("#div_minute:visible").length > 0;
        let isDivDayOfWeekVisible = $("#div_dayOfWeek:visible").length > 0;
        let isDivDayOfMonthVisible = $("#div_dayOfMonth:visible").length > 0;
        let hasErrors = false;

        // Validations
        if (isDivHourVisible) {
            const hour = parseInt($("#hour").val(), 10);
            if (isNaN(hour) || hour < 0 || hour > 23) {
                hasErrors = true;
                $("#error-hour").text("Hour must be between 0 and 23.").removeClass("hidden");
            }
        }

        if (isDivMinuteVisible) {
            const minute = parseInt($("#minute").val(), 10);
            if (isNaN(minute) || minute < 0 || minute > 59) {
                hasErrors = true;
                $("#error-minute").text("Minute must be between 0 and 59.").removeClass("hidden");
            }
        }
        if (hasErrors) {
            return;
        }

        // Value extractors
        const enabledValue = () => isEnabledVisible ? $("#enabled").val() : "true";
        const dryRunValue = () => isDryRunVisible ? $("#drpDryRun").val() : "false";
        const hourValue = () => isDivHourVisible ? $("#hour").val() : "0";
        const minuteValue = () => isDivMinuteVisible ? $("#minute").val() : "0";
        const dayOfWeekValue = () => isDivDayOfWeekVisible ? $("#dayOfWeek").val() : (!isDivDayOfWeekVisible ? "" : "Sunday");
        const dayOfMonthValue = () => isDivDayOfMonthVisible ? $("#dayOfMonth").val() : "0";
        // Generate config
        const template = configurationTemplate.getTemplate(
            value,
            enabledValue(),
            hourValue(),
            minuteValue(),
            dayOfWeekValue(),
            dayOfMonthValue(),
            dryRunValue()
        );
        $("#config-preview").text(template);
        $('#config-section').removeClass('hidden');
        AOS.refresh();
    });
    function showToast(message) {
        const toast = $('#copy-toast');
        toast.text(message);
        toast.addClass('show');

        setTimeout(() => {
            toast.removeClass('show');
        }, 3000);
    }
    $('#copyConfig').on('click', function () {
        const configText = $("#config-preview").text();
        navigator.clipboard.writeText(configText).then(() => {
            showToast("✅ Config copied to clipboard!");
        }).catch(() => {
            showToast("❌ Failed to copy config.");
        });
    });
});
