<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="View.ascx.cs" Inherits="Gafware.Modules.KeepAlive.View" %>
<style type="text/css">
    .KeepAlive_nocontent {
        display: none;
    }
</style>
<div id="KeepAlive_sessionWindow" class="KeepAlive_nocontent">
    <div id="KeepAlive_sessionWindow-content" class="KeepAlive_dialog-content">
        <div class="KeepAlive_body_padding">
            <p>
                Your session will time out in <span id="KeepAlive_timeRemaning">5</span> minutes due to inactivity.  Do you wish to continue?
            </p>
            <p>
                <asp:LinkButton ID="lnkYes" runat="server" CssClass="primaryButton" ToolTip="Refresh Session" Text="Yes" OnClick="lnkRefresh_Click" />
                <asp:LinkButton ID="lnkNo" runat="server" CssClass="secondaryButton" ToolTip="Don't Refresh Session" Text="No" OnClientClick="$('#KeepAlive_sessionWindow').dialog('close'); return false;" />
            </p>
        </div>
    </div>
</div>
<script type="text/javascript">
    $('#KeepAlive_sessionWindow').dialog({
        autoOpen: false,
        bgiframe: true,
        modal: true,
        width: 600,
        height: 275,
        dialogClass: 'dialog',
        title: 'Session Time Out',
        closeOnEsacpe: true
    });
    var sessionTimeoutTimer = 0;
    var alertTimeoutTimer = 0;
    var remainingTimer = 0;
    if (<%= IsLoggedIn %>) {
        sessionTimeoutTimer = setTimeout(function () { SessionTimeout(); }, <%= TimeOut %> * 60000);
        alertTimeoutTimer = setTimeout(function () { AlertSessionTimeout(); }, (<%= TimeOut %> - 5) * 60000);
    }
    $(document).ajaxStop(EndRequest);
    function EndRequest() {
        clearTimeout(sessionTimeoutTimer);
        clearTimeout(alertTimeoutTimer);
        HideAlertWindow();
        if (<%= IsLoggedIn %>) {
            sessionTimeoutTimer = setTimeout(function () { SessionTimeout(); }, <%= TimeOut %> * 60000);
            alertTimeoutTimer = setTimeout(function () { AlertSessionTimeout(); }, (<%= TimeOut %> - 5) * 60000);
        }
    }
    function SessionTimeout() {
        window.location = location.href = '/';
    }
    function AlertSessionTimeout() {
        $('#KeepAlive_sessionWindow').dialog('open');
        remainingTimer = setInterval('TimeRemaining();', 60000);
        //$('.ui-widget-overlay').hide();
    }
    function TimeRemaining() {
        var t = $('#KeepAlive_timeRemaning').text();
        t = t - 1;
        $('#KeepAlive_timeRemaning').text(t);
    }
    function HideAlertWindow() {
        try { $('#KeepAlive_sessionWindow').dialog('close'); } catch (e) { }
    }
</script>
