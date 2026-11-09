using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Win32;
using Path = System.IO.Path;

[assembly: AssemblyTitle("LAN Link")]
[assembly: AssemblyProduct("LAN Link")]
[assembly: AssemblyVersion("0.0.4.0")]
[assembly: AssemblyFileVersion("0.0.4.0")]
[assembly: AssemblyInformationalVersion("0.0.4")]

// Chinese text in the code and XAML doubles as the key into this table.
static class Lang
{
    public static volatile bool En;
    static readonly string SettingPath = Path.Combine(Sys.DataDir, "lang.txt");

    static readonly Dictionary<string, string> ToEn = new Dictionary<string, string>
    {
        { "LAN 互連", "LAN Link" },
        { "關閉", "Close" },
        { "最小化", "Minimize" },
        { "放大/還原", "Zoom / Restore" },
        { "這台電腦是", "This computer is" },
        { "電腦 A", "PC A" },
        { "電腦 B", "PC B" },
        { "網路卡", "Network adapter" },
        { "選項", "Options" },
        { "開放防火牆給對方", "Open firewall to peer" },
        { "只允許對方 IP 傳檔與 ping", "Only the peer's IP may transfer and ping" },
        { "允許對方遠端桌面", "Allow peer Remote Desktop" },
        { "對方可用這台的帳號密碼登入並操作這台", "The peer can sign in and control this PC with its password" },
        { "進階設定", "Advanced settings" },
        { "進階設定(IP / DNS)", "Advanced (IP / DNS)" },
        { "本機 IP", "This PC's IP" },
        { "子網路遮罩", "Subnet mask" },
        { "對方 IP", "Peer IP" },
        { "閘道(可留白)", "Gateway (opt.)" },
        { "閘道", "Gateway" },
        { "DNS(可留白)", "DNS (optional)" },
        { "多個請用逗號分隔", "Separate multiple servers with commas" },
        { "一鍵設定", "One-Click Setup" },
        { "還原設定", "Restore Settings" },
        { "傳送檔案", "Send files" },
        { "拖曳檔案或資料夾到這裡,或按一下選擇檔案", "Drag files or folders here, or click to choose files" },
        { "傳送檔案給對方", "Send files to peer" },
        { "把檔案或資料夾拖到這裡,就會傳給對方", "Drop files or folders here to send them to the peer" },
        { "取消", "Cancel" },
        { "測試連線", "Test Connection" },
        { "開啟", "Open" },
        { "開啟收件位置", "Open Inbox" },
        { "遠端桌面連到對方", "Remote Desktop" },
        { "測速", "Speed Test" },
        { "變更…", "Change…" },
        { "變更收件位置", "Change inbox location" },
        { "重置", "Reset" },
        { "設定", "Settings" },
        { "完成", "Done" },
        { "開機時自動開啟", "Open at startup" },
        { "登入 Windows 後在系統匣背景執行,可以直接收檔", "Runs in the system tray after sign-in, ready to receive files" },
        { "關閉視窗時直接退出", "Quit when the window closes" },
        { "關閉後不留在系統匣,也不再接收檔案", "Doesn't stay in the system tray or receive files after closing" },
        { "介面樣式", "Interface style" },
        { "視窗按鈕的位置和圖示", "Position and look of the window buttons" },
        { "展開視窗", "Show Window" },
        { "退出", "Quit" },
        { "LAN 互連仍在系統匣執行,可以繼續收檔。右鍵按圖示可以展開視窗或退出。", "LAN Link is still running in the system tray and can receive files. Right-click the icon to show the window or quit." },
        { "無法儲存設定:", "Can't save the setting: " },
        { "重置收件位置", "Reset inbox location to default" },
        { "收件位置(這台)", "Inbox (this PC)" },
        { "送達位置(對方)", "Destination (peer)" },
        { "送達位置", "Destination" },
        { "重置送達位置", "Reset the destination" },
        { "有指定時,傳出的檔案優先存到對方的這個資料夾。請輸入完整路徑,例如 D:\\收件。", "When set, files you send are saved to this folder on the peer first. Enter a full path, such as D:\\Inbox." },
        { "留白時存到對方的收件位置;對方沒設定過,就是對方 Windows 的「下載」資料夾。", "When empty, files go to the peer's inbox location, which is the peer's Windows Downloads folder unless the peer changed it." },
        { "請輸入對方電腦上的完整路徑,例如 D:\\收件", "Enter a full path on the peer, such as D:\\Inbox" },
        { "送達位置必須是完整路徑", "The destination must be a full path" },
        { "已傳送 {0} 個檔案到 {1}。", "Sent {0} file(s) to {1}." },
        { "對方沒有回應,請確認兩台都是新版工具。", "The peer did not respond. Make sure both PCs run the latest version." },
        { "瀏覽…", "Browse…" },
        { "瀏覽對方的資料夾", "Browse the peer's folders" },
        { "選擇對方的資料夾", "Choose a folder on the peer" },
        { "上一層", "Up" },
        { "選這個資料夾", "Select This Folder" },
        { "對方的磁碟", "Peer's drives" },
        { "這裡沒有子資料夾", "No subfolders here" },
        { "無法開啟這個資料夾:", "Can't open this folder: " },

        { "本機 IP 格式不正確", "This PC's IP is not valid" },
        { "子網路遮罩不正確", "The subnet mask is not valid" },
        { "本機 IP 不能是網段位址或廣播位址", "This PC's IP cannot be a network or broadcast address" },
        { "對方 IP 格式不正確", "The peer IP is not valid" },
        { "對方 IP 不能和本機相同", "The peer IP cannot match this PC's IP" },
        { "對方 IP 必須和本機在同一網段", "The peer IP must be on the same subnet as this PC" },
        { "閘道格式不正確", "The gateway is not valid" },
        { "閘道必須和本機在同一網段", "The gateway must be on the same subnet as this PC" },
        { "DNS 格式不正確", "The DNS address is not valid" },
        { "(未接線)", " (unplugged)" },
        { "(已接線)", " (connected)" },
        { "(已接線 · {0})", " (connected · {0})" },
        { "(已連線)", " (online)" },
        { "(已連線 · {0})", " (online · {0})" },
        { "對方電腦", "Other PC" },
        { "自動更新", "Automatic updates" },
        { "自動下載新版,在關閉程式或待命時安裝", "Downloads new versions and installs them on quit or while idle" },
        { "檢查更新", "Check Now" },
        { "正在檢查更新…", "Checking for updates…" },
        { "目前版本 v{0}", "Current version: v{0}" },
        { "已經是最新版本 v{0}({1} 檢查)", "v{0} is up to date (checked {1})" },
        { "正在下載新版 v{0}…", "Downloading v{0}…" },
        { "發現新版 v{0},按「立即更新」安裝。", "v{0} is available. Press “Update Now” to install it." },
        { "立即更新", "Update Now" },
        { "無法連線到 GitHub,請確認這台能上網後再試一次。", "Can't reach GitHub. Check that this PC is online, then try again." },
        { "GitHub 上沒有可用的更新。", "GitHub has no usable update." },
        { "正在安裝新版 v{0},程式會重新啟動…", "Installing v{0}. The app will restart…" },
        { "新版 v{0} 已下載,關閉程式時會自動安裝。", "v{0} is downloaded and installs when you quit." },
        { "更新失敗:{0}", "Update failed: {0}" },
        { "下載的檔案校驗碼不符,已取消更新。", "The download's checksum doesn't match, so the update was canceled." },
        { "下載的檔案版本不符,已取消更新。", "The download's version doesn't match, so the update was canceled." },
        { "新版沒有通過自我檢查,可能被防毒軟體攔住,已取消更新。", "The new version failed its self-test, or antivirus may have stopped it. The update was canceled." },
        { "新版無法啟動,可能被防毒軟體攔住,已換回目前的版本。", "The new version didn't start, and antivirus may have stopped it. The current version was put back." },
        { "這台的遠端桌面已開啟", "Remote Desktop is on for this PC" },
        { "這台的遠端桌面已允許,但服務沒有執行,按「一鍵設定」會啟動", "Remote Desktop is allowed, but its service isn't running. “One-Click Setup” starts it" },
        { "這台的遠端桌面目前是關閉的,按「一鍵設定」後才會開啟", "Remote Desktop is off for this PC. “One-Click Setup” turns it on" },
        { " 但這台的遠端桌面沒有成功開啟,請檢查 Windows 設定或公司政策。", " But Remote Desktop didn't turn on. Check Windows settings or company policy." },
        { "網路介面卡…", "Adapters…" },
        { "開啟 Windows 的網路連線設定", "Opens the Windows Network Connections window" },
        { "改成自動取得 IP", "Use Automatic IP" },
        { "不看備份,直接把這張網卡改回自動取得 IP 和 DNS", "Sets this adapter back to automatic IP and DNS, with or without a backup" },
        { "設定中…如果跳出權限視窗,請按「是」。", "Setting up… If a permission window appears, choose “Yes”." },
        { "已改成自動取得 IP。", "This adapter now uses an automatic IP." },
        { "無法改成自動取得 IP,請再試一次。", "Couldn't switch to an automatic IP. Please try again." },
        { "Wi-Fi 和熱點不用改,這個按鈕只用於網路線和 Thunderbolt。", "Wi-Fi and hotspots don't need this. It only applies to a cable or Thunderbolt." },
        { "手機 QR Code", "Phone QR Code" },
        { "檔案清單", "File List" },
        { "1. 掃描連上熱點", "1. Scan to join the hotspot" },
        { "2. 掃描開啟網頁", "2. Scan to open the page" },
        { "密碼…", "Password…" },
        { "熱點密碼", "Hotspot password" },
        { "儲存", "Save" },
        { "8 到 63 個英文、數字或符號。會一併改掉 Windows 行動熱點的密碼;熱點開著時會重新啟動,已連上的裝置要用新密碼重連。", "8 to 63 letters, digits or symbols. This also changes the Windows mobile hotspot password. A running hotspot restarts, so connected devices must rejoin with the new password." },
        { "密碼要 8 到 63 個英文、數字或符號。", "The password needs 8 to 63 letters, digits or symbols." },
        { "正在更新熱點密碼…", "Updating the hotspot password…" },
        { "熱點密碼已更新。", "The hotspot password is updated." },
        { "開網頁給對方下載", "Web Download" },
        { "對方沒裝工具、也沒設定時,用瀏覽器下載", "For a PC with no tool or setup: it downloads with a browser" },
        { "在另一台的瀏覽器輸入這個網址,對方不用裝工具,也不用改設定。", "Enter this address in the other PC's browser. It needs no tool and no setup." },
        { "加入檔案…", "Add Files…" },
        { "加入資料夾…", "Add Folder…" },
        { "清空清單", "Clear List" },
        { "停止分享", "Stop Sharing" },
        { "已停止分享。", "Sharing stopped." },
        { "選擇要給對方下載的檔案", "Choose files for the other PC to download" },
        { "選擇要給對方下載的資料夾", "Choose a folder for the other PC to download" },
        { "網頁下載只支援網路線、Thunderbolt 或 Wi-Fi 熱點,請先選對網卡。", "Web download works over a cable, Thunderbolt or this PC's Wi-Fi hotspot. Choose that adapter first." },
        { "另一台先連上熱點 {0}(密碼 {1}),再用瀏覽器輸入這個網址。", "On the other PC, join hotspot {0} (password {1}), then enter this address in a browser." },
        { "另一台先連上這台的熱點,再用瀏覽器輸入這個網址。", "On the other PC, join this PC's hotspot, then enter this address in a browser." },
        { "請先接上網路線或 Thunderbolt 線。", "Connect a network or Thunderbolt cable first." },
        { "正在取得 IP…連線後最多約一分鐘", "Getting an IP… Takes up to a minute after connecting" },
        { "還沒有檔案,把檔案或資料夾拖到這裡。", "No files yet. Drag files or folders here." },
        { "資料夾:{0}({1} 個檔案)", "Folder: {0} ({1} files)" },
        { "對方開始下載:{0}", "The other PC started downloading: {0}" },
        { "對方也能從網頁上傳檔案,會存到收件位置。", " The other device can also upload files from the page; they go to the inbox location." },
        { "上傳檔案給這台", "Upload files to this PC" },
        { "上傳完成 {0} 個,失敗 {1} 個。", "Uploaded {0}, failed {1}." },
        { "LAN 互連 檔案下載", "LAN Link Downloads" },
        { "對方還沒有選擇檔案,這個頁面會自動更新。", "No files have been shared yet. This page updates by itself." },
        { "下載資料夾 {0}.zip", "Download folder {0}.zip" },
        { "這個資料夾太大,無法打包,請逐檔下載:{0}", "This folder is too large to pack. Download its files one by one: {0}" },
        { "下載 LAN 互連工具", "Download the LAN Link tool" },
        { "設定…", "Edit…" },
        { "Wi-Fi 熱點", "Wi-Fi hotspot" },
        { "開啟 Wi-Fi 熱點", "Turn on Wi-Fi hotspot" },
        { "沒有共同 Wi-Fi 時,讓另一台直接連到這台", "Lets the other PC join this one when there is no shared Wi-Fi" },
        { "熱點名稱:{0}  密碼:{1}", "Hotspot: {0}  Password: {1}" },
        { "正在開啟熱點…", "Turning on the hotspot…" },
        { "正在關閉熱點…", "Turning off the hotspot…" },
        { "熱點已關閉。", "The hotspot is off." },
        { "熱點已開啟。請從另一台的 Wi-Fi 清單連上它,這台會自動填入對方 IP。", "The hotspot is on. Join it from the other PC's Wi-Fi list, and this PC fills in the peer IP automatically." },
        { "偵測到另一台 {0},請兩台都按一次「一鍵設定」。", "Found the other PC at {0}. Press “One-Click Setup” on both PCs." },
        { "退出時詢問是否還原", "Ask to restore on quit" },
        { "退出程式前先問要不要還原網路設定", "Asks whether to restore network settings before quitting" },
        { "每次退出自動還原", "Always restore on quit" },
        { "不再詢問,直接還原 IP、防火牆和熱點", "Restores the IP, firewall and hotspot without asking" },
        { "退出前要還原網路設定嗎?", "Restore network settings before quitting?" },
        { "會恢復原本的 IP,移除防火牆規則並關閉熱點。之後要傳檔,得再按一次「一鍵設定」。", "Puts back the original IP, removes the firewall rules and turns off the hotspot. Press “One-Click Setup” again before the next transfer." },
        { "還原並退出", "Restore and Quit" },
        { "直接退出", "Quit Only" },
        { "無法切換熱點:{0}。請確認 Wi-Fi 已開啟且這台已連上網路,或公司政策未停用熱點。", "Couldn't switch the hotspot: {0}. Check that Wi-Fi is on, this PC is online, and company policy allows hotspots." },
        { "這台 {0}。另一台連上這個熱點後,會自動填入對方 IP。", "This PC is {0}. The peer IP fills in automatically after the other PC joins this hotspot." },
        { "這台 {0}。兩台連同一個 Wi-Fi,在進階設定的「對方 IP」填另一台顯示的 IP。Wi-Fi 的 IP 不會被更改。", "This PC is {0}. Join the same Wi-Fi on both PCs and enter the other PC's IP as the peer IP in Advanced settings. The Wi-Fi IP stays unchanged." },
        { "傳輸內容不正確", "The received data is not valid" },
        { "對方中斷了測速", "The peer stopped the speed test" },
        { "傳給對方", "to peer" },
        { "從對方接收", "from peer" },
        { "對方沒有回應測速", "The peer did not answer the speed test" },
        { "找不到:", "Not found: " },
        { "檔案在傳送途中被改動:", "File changed while sending: " },
        { "對方沒有確認收到:", "The peer did not confirm receiving: " },
        { "不是 LAN 互連的傳輸", "This is not a LAN Link transfer" },
        { "對方傳來不安全的路徑,已拒絕", "Rejected an unsafe path from the peer" },
        { "對方中斷了傳輸", "The peer stopped the transfer" },
        { "這台是 Home 版,無法被遠端連線,但仍可連到別台", "Windows Home can't accept connections, but can still connect to others" },
        { "公司 IT 政策已停用這台的遠端桌面", "Your IT policy has disabled Remote Desktop on this PC" },
        { "已取消:變更網路設定需要系統管理員權限。", "Canceled: changing network settings needs administrator rights." },
        { "設定失敗(代碼 {0})", "Setup failed (code {0})" },
        { "不允許的指令", "Command not allowed" },
        { "{0} 執行失敗", "{0} failed" },
        { "找不到有線網卡,請確認網卡已啟用", "No wired adapter found. Check that it is enabled" },
        { "找不到有線網卡", "No wired adapter found" },
        { "網卡已切換,請兩台都按一次「一鍵設定」。", "Adapter changed. Press “One-Click Setup” on both PCs." },
        { "兩台各開一次這個工具,一台選 A、一台選 B,接上網路線或 Thunderbolt 線後按「一鍵設定」。", "Open this tool on both PCs, choose A on one and B on the other, connect the cable, then press “One-Click Setup”." },
        { "外觀:跟隨系統", "Appearance: system" },
        { "外觀:淺色", "Appearance: light" },
        { "外觀:深色", "Appearance: dark" },
        { "(按一下切換)", " (click to change)" },
        { "這台 {0}  ⇄  對方 {1}。不設閘道,不影響 Wi-Fi 上網。", "This PC {0}  ⇄  Peer {1}. No gateway is set, so Wi-Fi internet is not affected." },
        { "已設定完成,可以直接拖檔案傳送。", "Setup is complete. Drag files here to send them." },
        { "「{0}」已經在使用同一個網段,請打開進階設定換一個網段(例如 192.168.252.x)。", "“{0}” already uses this subnet. Choose another in Advanced settings (for example 192.168.252.x)." },
        { " 對方可以用遠端桌面連進這台。", " The peer can connect to this PC with Remote Desktop." },
        { "設定中…請在跳出的視窗按「是」。", "Setting up… Choose “Yes” in the window that appears." },
        { "完成!這台是 {0}。另一台也設定好後,把檔案拖到右邊的圖示就能傳送。{1}", "Done! This PC is {0}. Once the other PC is set up, drag files onto the icon to send them.{1}" },
        { "完成!這台是 {0}。防火牆未開放,這台只能傳出,收不到對方的檔案。{1}", "Done! This PC is {0}. The firewall is not open, so it can send files but not receive them.{1}" },
        { "測試中…", "Testing…" },
        { "連線成功({0} ms)。可以開始傳檔了。", "Connected ({0} ms). You can start sending files." },
        { "連不到對方。請確認:連接線已接好、兩台分別選 A 和 B、對方也按過「一鍵設定」且防火牆開關為開啟。", "Can't reach the peer. Check the cable, that one PC is A and the other B, and that the peer ran “One-Click Setup” with its firewall switch on." },
        { "已連線 · ", "Online · " },
        { "未連線 · ", "Offline · " },
        { "選擇要傳給對方的檔案", "Choose files to send to the peer" },
        { "傳送中", "Sending" },
        { "接收中", "Receiving" },
        { "準備傳送…", "Preparing to send…" },
        { "已傳送 {0} 個檔案。", "Sent {0} file(s)." },
        { "已取消傳送,對方不會留下未完成的檔案。", "Sending canceled. No unfinished files remain on the peer." },
        { "連不到對方。請確認連接線已接好、對方已按「一鍵設定」,且工具保持開啟。", "Can't reach the peer. Check the cable, and that the peer ran “One-Click Setup” and left this tool open." },
        { "傳送失敗:", "Sending failed: " },
        { "測速中,約需 10 秒…", "Testing speed, about 10 seconds…" },
        { "測速中…", "Testing speed…" },
        { "測速中 · ", "Testing speed · " },
        { "傳給對方 {0}\n從對方接收 {1}", "To peer {0}\nFrom peer {1}" },
        { "已取消測速。", "Speed test canceled." },
        { "測速失敗,請確認兩台都是新版工具。", "Speed test failed. Make sure both PCs run the latest version. " },
        { "對方完成測速。", "The peer finished a speed test." },
        { "收到 {0} 個檔案,存在 {1}。", "Received {0} file(s), saved in {1}." },
        { "接收中斷,未完成的檔案已清除。", "Receiving stopped. Unfinished files were removed. " },
        { "無法開啟收件位置:", "Can't open the inbox location: " },
        { "選擇收到的檔案要存放的資料夾", "Choose where to save received files" },
        { "之後收到的檔案會存到 ", "Received files will now be saved to " },
        { "無法儲存收件位置:", "Can't save the inbox location: " },
        { "檢查對方的遠端桌面…", "Checking the peer's Remote Desktop…" },
        { "對方沒有開放遠端桌面。請對方打開「允許對方遠端桌面」再按一次「一鍵設定」(Home 版無法被連線)。", "The peer has not allowed Remote Desktop. Ask them to turn on “Allow peer Remote Desktop” and run “One-Click Setup” again (Windows Home can't accept connections)." },
        { "已開啟遠端桌面,請輸入對方電腦的帳號密碼。連線期間對方的螢幕會鎖定。", "Remote Desktop is open. Sign in with the peer's account. The peer's screen locks while connected." },
        { "還原中…請在跳出的視窗按「是」。", "Restoring… Choose “Yes” in the window that appears." },
        { "已移除防火牆規則。這張網卡沒有備份,IP 未變更。", "Removed the firewall rules. This adapter has no backup, so its IP was not changed." },
        { "還原 IP 失敗,備份已保留,請再試一次。", "Restoring the IP failed. The backup was kept. Please try again." },
        { "已還原成原本的網路設定,並移除防火牆規則。", "Restored the original network settings and removed the firewall rules." }
    };

    static readonly Dictionary<string, string> ToZh = ToEn.ToDictionary(p => p.Value, p => p.Key);

    public static string L(string zh)
    {
        string en;
        return En && ToEn.TryGetValue(zh, out en) ? en : zh;
    }

    // For text already on screen, which may be in either language.
    public static string Swap(string shown)
    {
        string other;
        return (En ? ToEn : ToZh).TryGetValue(shown, out other) ? other : shown;
    }

    public static bool FormatsMatch()
    {
        return ToEn.All(p => Enumerable.Range(0, 3).All(i => p.Key.Contains("{" + i + "}") == p.Value.Contains("{" + i + "}")));
    }

    public static void Load()
    {
        string saved = "";
        try { saved = File.ReadAllText(SettingPath).Trim(); }
        catch (IOException) { }
        En = saved == "" ? System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName != "zh" : saved == "en";
    }

    public static void Save()
    {
        Directory.CreateDirectory(Sys.DataDir);
        File.WriteAllText(SettingPath, En ? "en" : "zh");
    }
}

class Config
{
    public bool Dhcp;
    public string Ip = "", Mask = "", Gateway = "";
    public string[] Dns = new string[0];
}

// Pure logic: no I/O, covered by --selftest.
static class Net
{
    public const string RuleName = "LanLink", ShareName = "LanLink", RdpRuleName = "LanLink-RDP";
    public const int TransferPort = 45450, RdpPort = 3389;
    const string RdpKey = "HKLM\\SYSTEM\\CurrentControlSet\\Control\\Terminal Server";

    public static bool IsIp(string s)
    {
        var p = s.Split('.');
        byte b;
        return p.Length == 4 && p.All(x => x.Length > 0 && x.All(char.IsDigit) && byte.TryParse(x, out b));
    }

    // A path that doesn't depend on the current drive or directory of the PC that uses it.
    public static bool IsFullFolder(string s)
    {
        return s.StartsWith("\\\\") || (s.Length >= 3 && char.IsLetter(s[0]) && s[1] == ':' && s[2] == '\\');
    }

    static uint ToUint(string ip)
    {
        return ip.Split('.').Aggregate(0u, (acc, x) => (acc << 8) | byte.Parse(x));
    }

    public static bool IsMask(string s)
    {
        if (!IsIp(s)) return false;
        uint inv = ~ToUint(s);
        return inv != 0xFFFFFFFF && (inv & (inv + 1)) == 0;
    }

    public static bool SameSubnet(string a, string b, string mask)
    {
        uint m = ToUint(mask);
        return (ToUint(a) & m) == (ToUint(b) & m);
    }

    public static string[] ParseDns(string s)
    {
        return s.Split(new[] { ',', ' ', ';' }, StringSplitOptions.RemoveEmptyEntries);
    }

    // Returns an error message, or null when valid.
    public static string Validate(Config c, string peer)
    {
        if (!IsIp(c.Ip)) return "本機 IP 格式不正確";
        if (!IsMask(c.Mask)) return "子網路遮罩不正確";
        uint host = ToUint(c.Ip) & ~ToUint(c.Mask);
        if (host == 0 || host == ~ToUint(c.Mask)) return "本機 IP 不能是網段位址或廣播位址";
        if (!IsIp(peer)) return "對方 IP 格式不正確";
        if (peer == c.Ip) return "對方 IP 不能和本機相同";
        if (!SameSubnet(c.Ip, peer, c.Mask)) return "對方 IP 必須和本機在同一網段";
        if (c.Gateway != "" && !IsIp(c.Gateway)) return "閘道格式不正確";
        if (c.Gateway != "" && !SameSubnet(c.Ip, c.Gateway, c.Mask)) return "閘道必須和本機在同一網段";
        if (!c.Dns.All(IsIp)) return "DNS 格式不正確";
        return null;
    }

    public static string AddressCommand(string name, Config c)
    {
        string q = "name=\"" + name + "\"";
        return c.Dhcp
            ? "interface ipv4 set address " + q + " source=dhcp"
            : string.Format("interface ipv4 set address {0} source=static address={1} mask={2} gateway={3}",
                q, c.Ip, c.Mask, c.Gateway == "" ? "none" : c.Gateway);
    }

    public static List<string> DnsCommands(string name, Config c)
    {
        string q = "name=\"" + name + "\"";
        var r = new List<string>();
        if (c.Dns.Length == 0)
            r.Add("interface ipv4 set dnsservers " + q + (c.Dhcp ? " source=dhcp" : " source=static address=none validate=no"));
        for (int i = 0; i < c.Dns.Length; i++)
            r.Add(i == 0
                ? "interface ipv4 set dnsservers " + q + " source=static address=" + c.Dns[0] + " validate=no"
                : "interface ipv4 add dnsservers " + q + " address=" + c.Dns[i] + " index=" + (i + 1) + " validate=no");
        return r;
    }

    public static string[] FirewallCommands(string peer, bool wifi = false)
    {
        string common = "advfirewall firewall add rule name=" + RuleName + " dir=in action=allow interfacetype=" + (wifi ? "wireless" : "lan") + " profile=any remoteip=" + peer;
        return new[]
        {
            common + " protocol=TCP localport=" + TransferPort,
            common + " protocol=icmpv4:8,any"
        };
    }

    public const string WebRuleName = "LanLink-Web", WebHotspotRuleName = "LanLink-WebHotspot";
    public const int WebPort = 45451;

    public static bool IsLinkLocal(string ip) { return ip.StartsWith("169.254."); }

    // Who may download: an unconfigured PC on a cable, or a client of this PC's hotspot.
    public static bool IsWebClient(string ip) { return IsLinkLocal(ip) || (ip.StartsWith("192.168.137.") && ip != HotspotIp); }

    static string RuleDelete(string rule) { return "advfirewall firewall delete rule name=" + rule; }

    public static List<string> WebRuleDeleteSteps()
    {
        return new List<string> { Step(false, "netsh", RuleDelete(WebRuleName)), Step(false, "netsh", RuleDelete(WebHotspotRuleName)) };
    }

    // An unconfigured PC falls back to a 169.254.x.x address, so this one has to be on that network too.
    public static List<string> WebSteps(string name, bool switchToAuto)
    {
        var r = switchToAuto ? ConfigSteps(name, new Config { Dhcp = true }, false) : new List<string>();
        r.Add(Step(false, "netsh", RuleDelete(WebRuleName)));
        r.Add(Step(true, "netsh", "advfirewall firewall add rule name=" + WebRuleName + " dir=in action=allow interfacetype=lan profile=any remoteip=169.254.0.0/16 protocol=TCP localport=" + WebPort));
        return r;
    }

    // The hotspot's address belongs to Windows, so only the firewall changes here.
    public static List<string> WebHotspotSteps()
    {
        return new List<string>
        {
            Step(false, "netsh", RuleDelete(WebHotspotRuleName)),
            Step(true, "netsh", "advfirewall firewall add rule name=" + WebHotspotRuleName + " dir=in action=allow interfacetype=wireless profile=any remoteip=192.168.137.0/24 protocol=TCP localport=" + WebPort)
        };
    }

    public static string FirewallDelete()
    {
        return "advfirewall firewall delete rule name=" + RuleName;
    }

    // Older versions could create this share. The feature is gone, so setup and restore clean it up.
    public static string ShareDelete()
    {
        return "share " + ShareName + " /delete /y";
    }

    static string RdpDenyCommand(int value)
    {
        return string.Format("add \"{0}\" /v fDenyTSConnections /t REG_DWORD /d {1} /f", RdpKey, value);
    }

    static string RdpRuleDelete()
    {
        return "advfirewall firewall delete rule name=" + RdpRuleName;
    }

    public static List<string> RdpEnableSteps(string peer, bool wifi = false)
    {
        return new List<string>
        {
            Step(false, "netsh", RdpRuleDelete()),
            Step(true, "reg", RdpDenyCommand(0)),
            Step(true, "netsh", "advfirewall firewall add rule name=" + RdpRuleName + " dir=in action=allow interfacetype=" + (wifi ? "wireless" : "lan") + " profile=any remoteip=" + peer + " protocol=TCP localport=" + RdpPort),
            Step(false, "net", "start TermService")
        };
    }

    public static string RdpStatus(bool denied, bool listening)
    {
        if (denied) return "這台的遠端桌面目前是關閉的,按「一鍵設定」後才會開啟";
        return listening ? "這台的遠端桌面已開啟" : "這台的遠端桌面已允許,但服務沒有執行,按「一鍵設定」會啟動";
    }

    // Turns RDP back off only if it was off before this tool enabled it.
    public static List<string> RdpDisableSteps(bool wasDenied)
    {
        var r = new List<string> { Step(false, "netsh", RdpRuleDelete()) };
        if (wasDenied) r.Add(Step(true, "reg", RdpDenyCommand(1)));
        return r;
    }

    const string WiredPrefix = "192.168.250.", ThunderboltPrefix = "192.168.251.";

    // Thunderbolt gets its own subnet so it never collides with the address kept on the wired adapter.
    public static string SubnetPrefix(Adapter a)
    {
        return a != null && a.Thunderbolt ? ThunderboltPrefix : WiredPrefix;
    }

    public static bool IsDefaultAddress(string ip)
    {
        return new[] { WiredPrefix + "1", WiredPrefix + "2", ThunderboltPrefix + "1", ThunderboltPrefix + "2" }.Contains(ip);
    }

    // Windows lists Thunderbolt and USB4 cable links as a normal Ethernet adapter, so only the description tells them apart.
    public static bool IsThunderbolt(string description)
    {
        string d = description.ToLowerInvariant();
        return d.Contains("usb4") || d.Contains("thunderbolt");
    }

    // A connected Thunderbolt link or hotspot replaces an unplugged choice. Other wired adapters don't, since they may be the office network.
    public static Adapter PickAdapter(List<Adapter> list, string currentId)
    {
        var keep = list.FirstOrDefault(a => a.Id == currentId);
        if (keep != null && keep.Up) return keep;
        return list.FirstOrDefault(a => a.Up && (a.Thunderbolt || a.Hotspot)) ?? keep ?? list.FirstOrDefault();
    }

    // Windows always gives its mobile hotspot this address and hands clients the rest of the /24.
    public const string HotspotIp = "192.168.137.1";

    // In the environment, because a quote in a password would break the command line, which other programs can also read.
    public const string HotspotPasswordVariable = "LANLINK_HOTSPOT_PASS";

    public static bool IsHotspotPassword(string s)
    {
        return s.Length >= 8 && s.Length <= 63 && s.All(c => c >= ' ' && c <= '~');
    }

    // Mobile hotspot is a WinRT API, which csc can't reach without extra references. op is start, stop, status or password.
    public static string HotspotScript(string op)
    {
        return string.Join(" ", new[]
        {
            "$ErrorActionPreference='Stop';",
            "try {",
            "[void][Windows.Networking.Connectivity.NetworkInformation,Windows.Networking.Connectivity,ContentType=WindowsRuntime];",
            "$T=[Windows.Networking.NetworkOperators.NetworkOperatorTetheringManager,Windows.Networking.NetworkOperators,ContentType=WindowsRuntime];",
            "$N=[Windows.Networking.Connectivity.NetworkInformation];",
            "$p=$N::GetInternetConnectionProfile();",
            "if(-not $p){$p=$N::GetConnectionProfiles()|Where-Object{$_.GetNetworkConnectivityLevel() -ne 'None'}|Select-Object -First 1};",
            "if(-not $p){'ERROR=NoConnection';exit};",
            "$m=$T::CreateFromConnectionProfile($p);",
            "if('" + op + "' -eq 'password'){",
            "Add-Type -AssemblyName System.Runtime.WindowsRuntime;",
            "$c=$m.GetCurrentAccessPointConfiguration();",
            "$c.Passphrase=$env:" + HotspotPasswordVariable + ";",
            "$a=[System.WindowsRuntimeSystemExtensions].GetMethods()|Where-Object{$_.Name -eq 'AsTask' -and $_.GetParameters().Count -eq 1 -and $_.GetParameters()[0].ParameterType.Name -eq 'IAsyncAction'}|Select-Object -First 1;",
            "$w=$a.Invoke($null,@($m.ConfigureAccessPointAsync($c)));",
            "if(-not $w.Wait(30000)){'ERROR=Timeout';exit};",
            "};",
            "if('" + op + "' -ne 'status' -and '" + op + "' -ne 'password'){",
            "Add-Type -AssemblyName System.Runtime.WindowsRuntime;",
            "$a=[System.WindowsRuntimeSystemExtensions].GetMethods()|Where-Object{$_.Name -eq 'AsTask' -and $_.GetParameters().Count -eq 1 -and $_.GetParameters()[0].ParameterType.Name -eq 'IAsyncOperation`1'}|Select-Object -First 1;",
            "$a=$a.MakeGenericMethod([Windows.Networking.NetworkOperators.NetworkOperatorTetheringOperationResult]);",
            "$w=$a.Invoke($null,@(if('" + op + "' -eq 'start'){$m.StartTetheringAsync()}else{$m.StopTetheringAsync()}));",
            "if(-not $w.Wait(30000)){'ERROR=Timeout';exit};",
            "'RESULT='+$w.Result.Status;",
            "};",
            "$c=$m.GetCurrentAccessPointConfiguration();",
            "'STATE='+$m.TetheringOperationalState;",
            "'SSID='+$c.Ssid;",
            "'PASS='+$c.Passphrase;",
            "} catch { 'ERROR='+$_.Exception.Message }"
        });
    }

    // Keeps the current peer while it is connected. With several clients the choice stays with the user.
    public static string HotspotPeer(IEnumerable<string> neighbors, string current)
    {
        var clients = neighbors.Where(n => n.StartsWith("192.168.137.") && n != HotspotIp && n != "192.168.137.255").Distinct().ToList();
        if (clients.Contains(current)) return current;
        return clients.Count == 1 ? clients[0] : null;
    }

    // The text phone cameras read as "join this Wi-Fi". These characters are special in the format.
    public static string WifiQrText(string ssid, string password)
    {
        Func<string, string> escape = s => string.Concat(s.Select(c => "\\;,\":".IndexOf(c) >= 0 ? "\\" + c : c.ToString()));
        return "WIFI:T:WPA;S:" + escape(ssid) + ";P:" + escape(password) + ";;";
    }

    public static Dictionary<string, string> ParseHotspot(string output)
    {
        var r = new Dictionary<string, string>();
        foreach (var line in output.Split('\r', '\n'))
        {
            int i = line.IndexOf('=');
            if (i > 0) r[line.Substring(0, i)] = line.Substring(i + 1);
        }
        return r;
    }

    // Null when the script's output means success.
    public static string HotspotError(Dictionary<string, string> r)
    {
        if (r.ContainsKey("ERROR")) return r["ERROR"];
        if (!r.ContainsKey("STATE")) return "No result";
        return r.ContainsKey("RESULT") && r["RESULT"] != "Success" ? r["RESULT"] : null;
    }

    // speedBps is the negotiated link speed; drivers report 0 or -1 when it is unknown.
    public static string LinkText(bool up, long speedBps, bool wifi = false)
    {
        if (!up) return Lang.L("(未接線)");
        if (speedBps <= 0) return Lang.L(wifi ? "(已連線)" : "(已接線)");
        string speed = speedBps >= 1000000000
            ? (speedBps / 1e9).ToString("0.##", System.Globalization.CultureInfo.InvariantCulture) + " Gbps"
            : speedBps / 1000000 + " Mbps";
        return string.Format(Lang.L(wifi ? "(已連線 · {0})" : "(已接線 · {0})"), speed);
    }

    public static string SpeedText(long bytes, long ms)
    {
        double seconds = Math.Max(ms, 1) / 1000.0;
        return string.Format("{0:F0} Mbps({1:F0} MB/s)", bytes * 8 / 1e6 / seconds, bytes / 1e6 / seconds);
    }

    // Home editions report EditionID Core, CoreSingleLanguage or CoreCountrySpecific and have no RDP host.
    public static bool IsHomeEdition(string editionId)
    {
        return editionId != null && editionId.StartsWith("Core", StringComparison.OrdinalIgnoreCase);
    }

    // One elevated step: a failing "must" step stops the batch and reports its output.
    public static string Step(bool must, string exe, string args)
    {
        return (must ? "!" : "?") + "\t" + exe + "\t" + args;
    }

    public static List<string> ConfigSteps(string name, Config c, bool skipAddress)
    {
        // netsh reports an error when DHCP is already on, so DHCP steps are best-effort and verified by the caller.
        var r = new List<string>();
        if (!skipAddress) r.Add(Step(!c.Dhcp, "netsh", AddressCommand(name, c)));
        r.AddRange(DnsCommands(name, c).Select(cmd => Step(!cmd.EndsWith("source=dhcp"), "netsh", cmd)));
        return r;
    }

    // Wi-Fi keeps the address it got from the router: a fixed IP here would cut the PC off the internet.
    public static List<string> AddressSteps(Adapter a, Config c, bool unchanged)
    {
        return a.Wireless ? new List<string>() : ConfigSteps(a.Name, c, unchanged);
    }

    public static string[] Serialize(Config c)
    {
        return new[] { c.Dhcp ? "dhcp" : "static", c.Ip, c.Mask, c.Gateway, string.Join(",", c.Dns) };
    }

    public static Config Deserialize(string[] l)
    {
        return new Config { Dhcp = l[0] == "dhcp", Ip = l[1], Mask = l[2], Gateway = l[3], Dns = ParseDns(l[4]) };
    }

    public static bool SameAddress(Config a, Config b)
    {
        return !a.Dhcp && !b.Dhcp && a.Ip == b.Ip && a.Mask == b.Mask && a.Gateway == b.Gateway;
    }

    // Paths come from the network: returns a clean relative path, or null when it could escape or alias anything.
    public static string SafeRelativePath(string rel)
    {
        var reserved = new[] { "CON", "PRN", "AUX", "NUL" }
            .Concat(Enumerable.Range(1, 9).SelectMany(i => new[] { "COM" + i, "LPT" + i }));
        var parts = rel.Split('/', '\\');
        foreach (var p in parts)
        {
            if (p == "" || p == "." || p == ".." || p.EndsWith(".") || p.EndsWith(" ")) return null;
            if (p.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0) return null;
            if (reserved.Contains(p.Split('.')[0].ToUpperInvariant())) return null;
        }
        return string.Join("\\", parts);
    }

    public static string UniquePath(string path, Func<string, bool> exists)
    {
        if (!exists(path)) return path;
        string dir = Path.GetDirectoryName(path), name = Path.GetFileNameWithoutExtension(path), ext = Path.GetExtension(path);
        for (int i = 1; ; i++)
        {
            var candidate = Path.Combine(dir, name + " (" + i + ")" + ext);
            if (!exists(candidate)) return candidate;
        }
    }

    public static int SelfTest()
    {
        var failures = new List<string>();
        Action<bool, string> check = (ok, what) => { if (!ok) failures.Add(what); };

        check(IsIp("192.168.1.1"), "valid ip");
        check(!IsIp("1"), "short ip");
        check(!IsIp("192.168.1.300"), "octet range");
        check(!IsIp("1.2.3.-4"), "negative octet");
        check(IsMask("255.255.255.0"), "valid mask");
        check(!IsMask("255.0.255.0"), "holey mask");
        check(!IsMask("0.0.0.0"), "zero mask");
        check(IsFullFolder(Sys.DownloadsFolder()), "finds the downloads folder");
        check(IsFullFolder("D:\\in") && IsFullFolder("\\\\pc\\share"), "full folder");
        check(!IsFullFolder("") && !IsFullFolder("D:in") && !IsFullFolder("\\in") && !IsFullFolder("in"), "relative folder");

        var c = new Config { Ip = "192.168.250.1", Mask = "255.255.255.0" };
        check(Validate(c, "192.168.250.2") == null, "good config");
        check(Validate(c, "192.168.250.1") != null, "peer equals self");
        check(Validate(c, "10.0.0.2") != null, "peer outside subnet");
        check(Validate(new Config { Ip = "192.168.250.0", Mask = c.Mask }, "192.168.250.2") != null, "network address");
        check(Validate(new Config { Ip = "192.168.250.255", Mask = c.Mask }, "192.168.250.2") != null, "broadcast address");
        check(Validate(new Config { Ip = c.Ip, Mask = c.Mask, Dns = new[] { "x" } }, "192.168.250.2") != null, "bad dns");
        check(Validate(new Config { Ip = c.Ip, Mask = c.Mask, Gateway = "10.0.0.1" }, "192.168.250.2") != null, "gateway outside subnet");

        check(AddressCommand("E", c) == "interface ipv4 set address name=\"E\" source=static address=192.168.250.1 mask=255.255.255.0 gateway=none", "static cmd");
        check(AddressCommand("E", new Config { Dhcp = true }) == "interface ipv4 set address name=\"E\" source=dhcp", "dhcp cmd");
        check(DnsCommands("E", c)[0].EndsWith("source=static address=none validate=no"), "static no dns");
        check(DnsCommands("E", new Config { Dhcp = true })[0].EndsWith("source=dhcp"), "dhcp dns");
        var two = DnsCommands("E", new Config { Dns = new[] { "1.1.1.1", "8.8.8.8" } });
        check(two.Count == 2 && two[1].Contains("add dnsservers") && two[1].Contains("index=2"), "two dns");
        var fw = FirewallCommands("192.168.250.2");
        check(fw.All(x => x.Contains("remoteip=192.168.250.2") && x.Contains("interfacetype=lan")), "firewall scoped to peer");
        check(fw.Any(x => x.Contains("localport=" + TransferPort)), "firewall opens transfer port");
        check(fw.All(x => !x.Contains("localport=445")), "firewall leaves file sharing closed");
        check(ConfigSteps("E", c, true).Count == 1, "skip unchanged address");
        check(ConfigSteps("E", new Config { Dhcp = true }, false).All(s => s.StartsWith("?")), "dhcp steps are best-effort");
        check(ConfigSteps("E", c, false).All(s => s.StartsWith("!")), "static steps must succeed");

        var rdpOn = RdpEnableSteps("192.168.250.2");
        check(rdpOn.Any(s => s.Contains("/d 0") && s.StartsWith("!\treg")), "rdp enable sets registry");
        check(rdpOn.Any(s => s.Contains("remoteip=192.168.250.2") && s.Contains("interfacetype=lan") && s.Contains("localport=3389")), "rdp rule scoped to peer");
        check(RdpDisableSteps(true).Any(s => s.Contains("/d 1")), "rdp off when it was off before");
        check(!RdpDisableSteps(false).Any(s => s.Contains("\treg\t")), "rdp stays on when user had it on");
        check(IsHomeEdition("Core") && IsHomeEdition("CoreSingleLanguage") && !IsHomeEdition("Professional") && !IsHomeEdition("Enterprise"), "home edition detection");
        check(LinkText(true, 1000000000) == "(已接線 · 1 Gbps)", "gigabit link text");
        check(LinkText(true, 2500000000) == "(已接線 · 2.5 Gbps)", "2.5G link text");
        check(LinkText(true, 10000000000) == "(已接線 · 10 Gbps)", "10G link text");
        check(SpeedText(125000000, 1000) == "1000 Mbps(125 MB/s)", "speed text");
        check(LinkText(true, 100000000) == "(已接線 · 100 Mbps)", "100M link text");
        check(LinkText(true, 10000000) == "(已接線 · 10 Mbps)", "10M link text");
        check(LinkText(true, -1) == "(已接線)", "unknown speed hidden");
        check(LinkText(false, 1000000000) == "(未接線)", "unplugged hides speed");
        check(LinkText(true, 20000000000) == "(已接線 · 20 Gbps)", "20G link text");
        check(IsThunderbolt("USB4(TM) P2P Network Adapter") && IsThunderbolt("Thunderbolt(TM) Networking"), "thunderbolt adapter recognized");
        check(!IsThunderbolt("Intel(R) Ethernet Connection (24) I219-LM"), "wired adapter is not thunderbolt");
        var wired = new Adapter { Name = "E", Id = "w", Description = "Intel(R) Ethernet", Up = false };
        var tb = new Adapter { Name = "E 4", Id = "t", Description = "USB4(TM) P2P Network Adapter", Up = true };
        var office = new Adapter { Name = "E 2", Id = "o", Description = "Realtek Ethernet", Up = true };
        check(tb.ToString() == "Thunderbolt(已接線)" && wired.ToString() == "E(未接線)", "adapter labels");
        check(SubnetPrefix(tb) == "192.168.251." && SubnetPrefix(wired) == "192.168.250." && SubnetPrefix(null) == "192.168.250.", "subnet per adapter");
        check(IsDefaultAddress("192.168.251.2") && !IsDefaultAddress("192.168.252.1") && !IsDefaultAddress("192.168.250.11"), "default addresses");
        check(PickAdapter(new List<Adapter> { tb, wired }, "w") == tb, "thunderbolt replaces unplugged adapter");
        check(PickAdapter(new List<Adapter> { office, wired }, "w") == wired, "office network never replaces the choice");
        check(PickAdapter(new List<Adapter> { office, tb }, "o") == office, "connected choice is kept");
        check(PickAdapter(new List<Adapter> { office, wired }, null) == office, "first adapter without a choice");
        var wifiCard = new Adapter { Name = "Wi-Fi", Id = "f", Description = "Intel(R) Wi-Fi 7", Up = true, Wireless = true, Speed = 1100000000 };
        check(wifiCard.ToString() == "Wi-Fi(已連線 · 1.1 Gbps)", "wifi label");
        var hotspot = new Adapter { Name = "L 9", Id = "h", Description = "Microsoft Wi-Fi Direct Virtual Adapter #2", Up = true, Wireless = true, Ip = HotspotIp };
        var joined = new Adapter { Name = "Wi-Fi", Id = "f", Description = "Intel(R) Wi-Fi 7", Up = true, Wireless = true, Ip = "192.168.137.20" };
        check(hotspot.Hotspot && !hotspot.HotspotClient && hotspot.ToString() == "Wi-Fi 熱點(已連線)", "hotspot adapter");
        check(joined.HotspotClient && !joined.Hotspot && !wifiCard.Hotspot && !wifiCard.HotspotClient, "hotspot client");
        check(AddressSteps(hotspot, c, false).Count == 0, "hotspot address is never changed");
        check(PickAdapter(new List<Adapter> { hotspot, wired }, "w") == hotspot, "hotspot replaces unplugged adapter");
        var seen = new[] { "192.168.137.1", "192.168.137.20", "192.168.137.255", "10.0.0.9" };
        check(HotspotPeer(seen, "") == "192.168.137.20", "hotspot client found");
        check(HotspotPeer(new[] { "192.168.137.1", "192.168.137.255" }, "") == null, "no hotspot client");
        check(HotspotPeer(new[] { "192.168.137.20", "192.168.137.21" }, "") == null, "several clients are not guessed");
        check(HotspotPeer(new[] { "192.168.137.20", "192.168.137.21" }, "192.168.137.21") == "192.168.137.21", "current client kept");
        check(HotspotPeer(seen, "192.168.137.99") == "192.168.137.20", "client with a new address");
        var parsed = ParseHotspot("STATE=On\r\nSSID=My Net\r\nPASS=a=b\r\n");
        check(parsed["SSID"] == "My Net" && parsed["PASS"] == "a=b" && HotspotError(parsed) == null, "hotspot output");
        check(HotspotError(ParseHotspot("RESULT=WiFiDeviceOff\r\nSTATE=Off")) == "WiFiDeviceOff", "hotspot refused");
        check(HotspotError(ParseHotspot("ERROR=NoConnection")) == "NoConnection" && HotspotError(ParseHotspot("")) != null, "hotspot failed");
        check(!HotspotScript("status").Contains("\"") && HotspotScript("start").Contains("'start' -ne 'status'"), "hotspot script quoting");
        check(HotspotScript("password").Contains("$env:" + HotspotPasswordVariable), "hotspot password comes from the environment");
        check(IsHotspotPassword("12345678") && IsHotspotPassword("a b\"c'd$e") && IsHotspotPassword(new string('x', 63)), "hotspot password accepted");
        check(!IsHotspotPassword("1234567") && !IsHotspotPassword(new string('x', 64)) && !IsHotspotPassword("密碼密碼密碼密碼"), "hotspot password refused");
        check(AddressSteps(wifiCard, c, false).Count == 0, "wifi address is never changed");
        check(AddressSteps(wired, c, false).Count > 0, "wired address is set");
        check(FirewallCommands("10.0.0.2", true).All(x => x.Contains("remoteip=10.0.0.2") && x.Contains("interfacetype=wireless")), "wifi firewall scoped to peer");
        check(RdpEnableSteps("10.0.0.2", true).Any(s => s.Contains("remoteip=10.0.0.2") && s.Contains("interfacetype=wireless")), "wifi rdp rule scoped to peer");

        var back = Deserialize(Serialize(new Config { Ip = "10.0.0.5", Mask = "255.0.0.0", Gateway = "10.0.0.1", Dns = new[] { "1.1.1.1", "8.8.8.8" } }));
        check(back.Ip == "10.0.0.5" && back.Gateway == "10.0.0.1" && back.Dns.Length == 2 && !back.Dhcp, "backup round trip");
        check(Deserialize(Serialize(new Config { Dhcp = true })).Dhcp, "dhcp round trip");

        foreach (var bad in new[] { "..\\x", "a\\..\\..\\x", "../x", "C:\\x", "\\\\srv\\x", "\\x", "a:stream", "NUL", "con.txt", "COM1", "a.", "a ", "", "a\\\\b" })
            check(SafeRelativePath(bad) == null, "must reject path: " + bad);
        check(SafeRelativePath("a/b\\c.txt") == "a\\b\\c.txt", "normalizes path");
        check(SafeRelativePath("資料夾\\console.txt") == "資料夾\\console.txt", "keeps ordinary names");
        check(UniquePath("C:\\a\\x.txt", p => p == "C:\\a\\x.txt" || p == "C:\\a\\x (1).txt") == "C:\\a\\x (2).txt", "unique path");

        try { Transfer.SelfTest(check); }
        catch (Exception ex) { failures.Add("transfer test threw: " + ex.Message); }
        try { WebShare.SelfTest(check); }
        catch (Exception ex) { failures.Add("web test threw: " + ex.Message); }

        Qr.SelfTest(check);
        Updater.SelfTest(check);
        check(RdpStatus(true, false).Contains("關閉") && RdpStatus(true, true).Contains("關閉"), "rdp denied reads as off");
        check(RdpStatus(false, true) == "這台的遠端桌面已開啟" && RdpStatus(false, false).Contains("服務"), "rdp on or service stopped");
        check(WifiQrText("My;Net", "a\"b:c\\d") == "WIFI:T:WPA;S:My\\;Net;P:a\\\"b\\:c\\\\d;;", "wifi qr text");

        var webAuto = WebSteps("E", true);
        check(webAuto.Any(x => x.Contains("source=dhcp")) && webAuto.Any(x => x.Contains("remoteip=169.254.0.0/16") && x.Contains("interfacetype=lan") && x.Contains("localport=45451")), "web setup");
        check(!WebSteps("E", false).Any(x => x.Contains("source=")), "web setup keeps an automatic address");
        check(IsLinkLocal("169.254.11.93") && !IsLinkLocal("192.168.250.1"), "link-local address");
        var webHotspot = WebHotspotSteps();
        check(webHotspot.Any(x => x.Contains("remoteip=192.168.137.0/24") && x.Contains("interfacetype=wireless") && x.Contains("localport=45451")), "web over hotspot opens only the hotspot network");
        check(!webHotspot.Any(x => x.Contains("source=") || x.Contains("169.254")), "web over hotspot keeps the hotspot address");
        check(IsWebClient("169.254.1.2") && IsWebClient("192.168.137.20"), "web clients accepted");
        check(!IsWebClient(HotspotIp) && !IsWebClient("10.61.212.244") && !IsWebClient("192.168.250.2") && !IsWebClient("127.0.0.1"), "web clients refused");
        check(WebRuleDeleteSteps().Any(x => x.Contains(WebRuleName)) && WebRuleDeleteSteps().Any(x => x.Contains(WebHotspotRuleName)), "web rules removed together");

        check(Lang.FormatsMatch(), "translations keep their format placeholders");
        Lang.En = true;
        check(Lang.L("測速") == "Speed Test", "english lookup");
        check(Lang.L("not in table") == "not in table", "unknown text passes through");
        check(Lang.Swap("測速") == "Speed Test", "swap to english");
        check(LinkText(true, 1000000000) == " (connected · 1 Gbps)", "english link text");
        Lang.En = false;
        check(Lang.L("測速") == "測速", "chinese stays as is");
        check(Lang.Swap("Speed Test") == "測速", "swap back to chinese");

        foreach (var f in failures) Console.Error.WriteLine("FAIL: " + f);
        return failures.Count == 0 ? 0 : 1;
    }
}

class TransferItem
{
    public string Full, Rel;
    public long Length; // -1 = directory
}

// Wire format: magic, send-to folder text ("" = the receiver's inbox), int64 total bytes, int32 item count, then per item
// int32 path length, UTF-8 path, int64 length (-1 = directory), data. The receiver answers one ack byte per item.
// A send-to folder is answered first: bool ok, followed by the error text when it can't be used.
static class Transfer
{
    static readonly byte[] Magic = Encoding.ASCII.GetBytes("LANLINK2");
    static readonly byte[] SpeedMagic = Encoding.ASCII.GetBytes("LANSPEED");
    static readonly byte[] FoldersMagic = Encoding.ASCII.GetBytes("LANDIRS1");
    const int MaxPathBytes = 8192, BufferSize = 1 << 20;
    public const string PartSuffix = ".lanlink-part";
    static int speedMs = 5000; // Duration of each speed test direction; the self-test shortens it.

    // Speed test frames: int32 length, then data, ending with length 0. Data stays in memory so disks don't affect the result.
    static long SendFrames(Stream s, Action<double> progress)
    {
        var w = new BinaryWriter(s);
        var buf = new byte[BufferSize];
        new Random().NextBytes(buf);
        long sent = 0;
        var clock = Stopwatch.StartNew();
        while (clock.ElapsedMilliseconds < speedMs)
        {
            w.Write(buf.Length);
            w.Write(buf);
            sent += buf.Length;
            progress((double)clock.ElapsedMilliseconds / speedMs);
        }
        w.Write(0);
        w.Flush();
        return sent;
    }

    static long ReadFrames(Stream s, Action<double> progress)
    {
        var r = new BinaryReader(s);
        var buf = new byte[BufferSize];
        long received = 0;
        var clock = Stopwatch.StartNew();
        while (true)
        {
            int length = r.ReadInt32();
            if (length == 0) return received;
            if (length < 0 || length > BufferSize) throw new InvalidDataException("傳輸內容不正確");
            for (int left = length; left > 0; )
            {
                int n = r.Read(buf, 0, left);
                if (n == 0) throw new EndOfStreamException("對方中斷了測速");
                left -= n;
                received += n;
            }
            progress(Math.Min(1, (double)clock.ElapsedMilliseconds / speedMs));
        }
    }

    // Returns bytes and milliseconds as { sent, sentMs, received, receivedMs }.
    public static long[] SpeedTest(Stream s, Action<string, double> progress)
    {
        s.Write(SpeedMagic, 0, SpeedMagic.Length);
        var clock = Stopwatch.StartNew();
        long sent = SendFrames(s, f => progress("傳給對方", f));
        // Timed through the ack, because writes return while data is still in socket buffers.
        if (s.ReadByte() != 1) throw new IOException("對方沒有回應測速");
        long sentMs = clock.ElapsedMilliseconds;
        clock.Restart();
        long received = ReadFrames(s, f => progress("從對方接收", f));
        return new[] { sent, sentMs, received, clock.ElapsedMilliseconds };
    }

    static void WriteText(BinaryWriter w, string text)
    {
        var data = Encoding.UTF8.GetBytes(text);
        w.Write(data.Length);
        w.Write(data);
    }

    static string ReadText(BinaryReader r)
    {
        int length = r.ReadInt32();
        if (length < 0 || length > MaxPathBytes) throw new InvalidDataException("傳輸內容不正確");
        var data = r.ReadBytes(length);
        if (data.Length != length) throw new EndOfStreamException();
        return Encoding.UTF8.GetString(data);
    }

    // The visible folders inside folder, or the drives when folder is "". A missing folder falls back to its nearest
    // parent, because a typed send-to folder doesn't exist until the first file arrives.
    static List<string> ListFolders(ref string folder)
    {
        while (folder != "" && !Directory.Exists(folder)) folder = Path.GetDirectoryName(folder) ?? "";
        if (folder == "") return DriveInfo.GetDrives().Where(d => d.IsReady).Select(d => d.Name).ToList();
        return new DirectoryInfo(folder).GetDirectories()
            .Where(d => (d.Attributes & (FileAttributes.Hidden | FileAttributes.System)) == 0)
            .Select(d => d.Name).OrderBy(n => n, StringComparer.CurrentCultureIgnoreCase).ToList();
    }

    // Folders request: magic, folder text. Reply: bool ok, then the error text, or the folder shown, int32 count and names.
    // Returns the folder shown followed by the names.
    public static string[] PeerFolders(Stream s, string folder)
    {
        var w = new BinaryWriter(s);
        w.Write(FoldersMagic);
        WriteText(w, folder);
        w.Flush();
        var r = new BinaryReader(s);
        bool ok = r.ReadBoolean();
        string first = ReadText(r);
        if (!ok) throw new Exception(first);
        int count = r.ReadInt32();
        if (count < 0) throw new InvalidDataException("傳輸內容不正確");
        var result = new List<string> { first };
        for (int i = 0; i < count; i++) result.Add(ReadText(r));
        return result.ToArray();
    }

    public static List<TransferItem> Collect(IEnumerable<string> paths)
    {
        var items = new List<TransferItem>();
        foreach (var raw in paths)
        {
            string p = raw.TrimEnd('\\');
            if (p.EndsWith(":")) p = raw; // drive root keeps its backslash
            string parent = Path.GetDirectoryName(p) ?? p;
            Func<string, string> rel = full => full.Substring(parent.Length).TrimStart('\\');
            if (Directory.Exists(p))
            {
                // ponytail: AllDirectories follows junctions; a loop ends in a visible path-too-long error, skip reparse points if that bites.
                foreach (var d in new[] { p }.Concat(Directory.GetDirectories(p, "*", SearchOption.AllDirectories)))
                    if (rel(d) != "") items.Add(new TransferItem { Full = d, Rel = rel(d), Length = -1 });
                foreach (var f in Directory.GetFiles(p, "*", SearchOption.AllDirectories))
                    items.Add(new TransferItem { Full = f, Rel = rel(f), Length = new FileInfo(f).Length });
            }
            else if (File.Exists(p))
                items.Add(new TransferItem { Full = p, Rel = rel(p), Length = new FileInfo(p).Length });
            else
                throw new FileNotFoundException(Lang.L("找不到:") + p);
        }
        return items;
    }

    public static void Send(Stream s, List<TransferItem> items, Action<string, long, long> progress, string sendTo = "")
    {
        var w = new BinaryWriter(s);
        long total = items.Where(i => i.Length > 0).Sum(i => i.Length), done = 0;
        w.Write(Magic);
        WriteText(w, sendTo);
        w.Write(total);
        w.Write(items.Count);
        w.Flush();
        if (sendTo != "")
        {
            var r = new BinaryReader(s);
            if (!r.ReadBoolean()) throw new IOException(ReadText(r));
        }
        var buf = new byte[BufferSize];
        foreach (var item in items)
        {
            var rel = Encoding.UTF8.GetBytes(item.Rel);
            w.Write(rel.Length);
            w.Write(rel);
            w.Write(item.Length);
            w.Flush();
            if (item.Length >= 0)
            {
                using (var f = new FileStream(item.Full, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    for (long left = item.Length; left > 0; )
                    {
                        int n = f.Read(buf, 0, (int)Math.Min(buf.Length, left));
                        if (n == 0) throw new IOException(Lang.L("檔案在傳送途中被改動:") + item.Rel);
                        s.Write(buf, 0, n);
                        left -= n;
                        done += n;
                        progress(item.Rel, done, total);
                    }
                }
            }
            if (s.ReadByte() != 1) throw new IOException(Lang.L("對方沒有確認收到:") + item.Rel);
        }
    }

    // Returns the number of files received, -1 when the peer ran a speed test instead, or -3 for a folders request.
    // A send-to folder from the sender wins over root; savedTo is told about it.
    // Never overwrites: a same-named file gets " (1)".
    public static int Receive(Stream s, string root, Action<string, long, long> progress, Action<string> savedTo = null)
    {
        var r = new BinaryReader(s);
        var head = r.ReadBytes(Magic.Length);
        if (head.SequenceEqual(SpeedMagic))
        {
            ReadFrames(s, f => { });
            s.WriteByte(1);
            SendFrames(s, f => { });
            return -1;
        }
        if (head.SequenceEqual(FoldersMagic))
        {
            string folder = ReadText(r), error = null;
            List<string> names = null;
            try { names = ListFolders(ref folder); }
            catch (Exception ex) { error = ex.Message; }
            var w = new BinaryWriter(s);
            w.Write(error == null);
            WriteText(w, error ?? folder);
            if (error == null)
            {
                w.Write(names.Count);
                foreach (var name in names) WriteText(w, name);
            }
            w.Flush();
            return -3;
        }
        if (!head.SequenceEqual(Magic)) throw new InvalidDataException("不是 LAN 互連的傳輸");
        string sendTo = ReadText(r);
        long total = r.ReadInt64(), done = 0;
        int count = r.ReadInt32(), files = 0;
        if (count < 0) throw new InvalidDataException("傳輸內容不正確");
        if (sendTo != "")
        {
            var w = new BinaryWriter(s);
            try
            {
                if (!Net.IsFullFolder(sendTo)) throw new InvalidDataException("送達位置必須是完整路徑");
                root = Path.GetFullPath(sendTo);
                Directory.CreateDirectory(root);
            }
            catch (Exception ex)
            {
                w.Write(false);
                WriteText(w, ex.Message);
                w.Flush();
                throw;
            }
            w.Write(true);
            w.Flush();
            if (savedTo != null) savedTo(root);
        }

        string rootFull = Path.GetFullPath(root).TrimEnd('\\') + "\\";
        Directory.CreateDirectory(rootFull);
        var buf = new byte[BufferSize];
        for (int i = 0; i < count; i++)
        {
            int pathBytes = r.ReadInt32();
            if (pathBytes <= 0 || pathBytes > MaxPathBytes) throw new InvalidDataException("傳輸內容不正確");
            var pathData = r.ReadBytes(pathBytes);
            if (pathData.Length != pathBytes) throw new EndOfStreamException();
            string rel = Net.SafeRelativePath(Encoding.UTF8.GetString(pathData));
            long length = r.ReadInt64();
            if (rel == null || length < -1) throw new InvalidDataException("對方傳來不安全的路徑,已拒絕");

            string target = Path.GetFullPath(Path.Combine(rootFull, rel));
            if (!target.StartsWith(rootFull, StringComparison.OrdinalIgnoreCase)) throw new InvalidDataException("對方傳來不安全的路徑,已拒絕");

            if (length == -1)
                Directory.CreateDirectory(target);
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(target));
                target = Net.UniquePath(target, p => File.Exists(p) || Directory.Exists(p));
                string part = target + PartSuffix;
                try
                {
                    using (var f = new FileStream(part, FileMode.Create, FileAccess.Write))
                    {
                        for (long left = length; left > 0; )
                        {
                            int n = r.Read(buf, 0, (int)Math.Min(buf.Length, left));
                            if (n == 0) throw new EndOfStreamException("對方中斷了傳輸");
                            f.Write(buf, 0, n);
                            left -= n;
                            done += n;
                            progress(rel, done, total);
                        }
                    }
                    File.Move(part, target);
                }
                catch
                {
                    File.Delete(part);
                    throw;
                }
                files++;
            }
            s.WriteByte(1);
        }
        return files;
    }

    public static void SelfTest(Action<bool, string> check)
    {
        string tmp = Path.Combine(Path.GetTempPath(), "LanLinkTest-" + Guid.NewGuid().ToString("N"));
        string src = Path.Combine(tmp, "src"), dst = Path.Combine(tmp, "dst");
        Action<string, long, long> quiet = (a, b, c) => { };
        try
        {
            Directory.CreateDirectory(Path.Combine(src, "資料夾", "空的"));
            var big = new byte[3 * BufferSize + 17];
            new Random(1).NextBytes(big);
            File.WriteAllBytes(Path.Combine(src, "資料夾", "大檔.bin"), big);
            File.WriteAllBytes(Path.Combine(src, "empty.txt"), new byte[0]);
            var items = Collect(new[] { Path.Combine(src, "資料夾"), Path.Combine(src, "empty.txt") });

            string savedTo = null;
            Action<string> sendOnce = sendTo =>
            {
                var l = new TcpListener(IPAddress.Loopback, 0);
                l.Start();
                int port = ((IPEndPoint)l.LocalEndpoint).Port;
                var rx = Task.Run(() =>
                {
                    using (var c = l.AcceptTcpClient()) Receive(c.GetStream(), dst, quiet, folder => savedTo = folder);
                });
                using (var c = new TcpClient())
                {
                    c.Connect(IPAddress.Loopback, port);
                    Send(c.GetStream(), items, quiet, sendTo);
                }
                rx.Wait();
                l.Stop();
            };
            sendOnce("");
            check(File.ReadAllBytes(Path.Combine(dst, "資料夾", "大檔.bin")).SequenceEqual(big), "transfer keeps bytes");
            check(Directory.Exists(Path.Combine(dst, "資料夾", "空的")), "transfer keeps empty folder");
            check(new FileInfo(Path.Combine(dst, "empty.txt")).Length == 0, "transfer keeps empty file");
            sendOnce("");
            check(File.Exists(Path.Combine(dst, "資料夾", "大檔 (1).bin")), "second send does not overwrite");
            check(savedTo == null, "no send-to folder uses the inbox");

            // A send-to folder wins over the inbox; one that can't be used is refused, not swapped for the inbox.
            string chosen = Path.Combine(tmp, "chosen");
            int inboxFiles = Directory.GetFiles(dst).Length;
            sendOnce(chosen);
            check(File.Exists(Path.Combine(chosen, "empty.txt")) && savedTo == Path.GetFullPath(chosen), "send-to folder is used");
            check(Directory.GetFiles(dst).Length == inboxFiles, "send-to folder leaves the inbox alone");
            bool refused = false;
            try { sendOnce("relative"); } catch (IOException) { refused = true; }
            check(refused && Directory.GetFiles(dst).Length == inboxFiles, "refuses a relative send-to folder");

            BinaryWriter w;
            speedMs = 200;
            var speedListener = new TcpListener(IPAddress.Loopback, 0);
            speedListener.Start();
            var speedServer = Task.Run(() =>
            {
                using (var c = speedListener.AcceptTcpClient()) return Receive(c.GetStream(), dst, quiet);
            });
            long[] speed;
            using (var c = new TcpClient())
            {
                c.Connect(IPAddress.Loopback, ((IPEndPoint)speedListener.LocalEndpoint).Port);
                speed = SpeedTest(c.GetStream(), (a, b) => { });
            }
            check(speedServer.Result == -1, "speed test is not a file transfer");
            speedListener.Stop();
            check(speed[0] > 0 && speed[1] >= 200 && speed[2] > 0 && speed[3] > 0, "speed test measures both directions");

            var hugeFrame = new MemoryStream();
            w = new BinaryWriter(hugeFrame);
            w.Write(SpeedMagic); w.Write(int.MaxValue);
            hugeFrame.Position = 0;
            bool frameRejected = false;
            try { Receive(hugeFrame, dst, quiet); } catch (InvalidDataException) { frameRejected = true; }
            check(frameRejected, "rejects oversized speed test frame");

            Func<Func<Stream, string[]>, string[]> exchange = ask =>
            {
                var l = new TcpListener(IPAddress.Loopback, 0);
                l.Start();
                var rx = Task.Run(() =>
                {
                    using (var c = l.AcceptTcpClient()) return Receive(c.GetStream(), dst, quiet);
                });
                try
                {
                    using (var c = new TcpClient())
                    {
                        c.Connect(IPAddress.Loopback, ((IPEndPoint)l.LocalEndpoint).Port);
                        return ask(c.GetStream());
                    }
                }
                finally
                {
                    check(rx.Result == -3, "folders request is not a file transfer");
                    l.Stop();
                }
            };
            string folders = Path.Combine(tmp, "folders");
            Directory.CreateDirectory(Path.Combine(folders, "sub"));
            Directory.CreateDirectory(Path.Combine(folders, "hide")).Attributes |= FileAttributes.Hidden;
            var shown = exchange(s => PeerFolders(s, folders));
            check(shown[0] == folders && shown.Skip(1).SequenceEqual(new[] { "sub" }), "lists visible peer folders");
            check(exchange(s => PeerFolders(s, Path.Combine(folders, "gone", "deeper")))[0] == folders, "missing folder falls back to its parent");
            var drives = exchange(s => PeerFolders(s, ""));
            check(drives[0] == "" && drives.Length > 1, "lists peer drives");

            // Hostile sender: a traversal path must not create anything outside the inbox.
            var evil = new MemoryStream();
            w = new BinaryWriter(evil);
            w.Write(Magic); w.Write(0); w.Write(1L); w.Write(1);
            var evilPath = Encoding.UTF8.GetBytes("..\\escaped.txt");
            w.Write(evilPath.Length); w.Write(evilPath); w.Write(1L); w.Write((byte)65);
            evil.Position = 0;
            bool rejected = false;
            try { Receive(evil, dst, quiet); } catch (InvalidDataException) { rejected = true; }
            check(rejected && !File.Exists(Path.Combine(tmp, "escaped.txt")), "rejects traversal");

            // Cut off mid-file: no partial file may remain.
            var cut = new MemoryStream();
            w = new BinaryWriter(cut);
            w.Write(Magic); w.Write(0); w.Write(10L); w.Write(1);
            var cutPath = Encoding.UTF8.GetBytes("cut.bin");
            w.Write(cutPath.Length); w.Write(cutPath); w.Write(10L); w.Write((byte)65);
            cut.Position = 0;
            bool threw = false;
            try { Receive(cut, dst, quiet); } catch (EndOfStreamException) { threw = true; }
            check(threw && !Directory.GetFiles(dst).Any(f => f.Contains("cut.bin")), "truncated transfer leaves nothing");
        }
        finally
        {
            if (Directory.Exists(tmp)) Directory.Delete(tmp, true);
        }
    }
}

class Adapter
{
    public string Name, Id, Description, Ip, Mask;
    public bool Up, Wireless;
    public long Speed;
    public bool Thunderbolt { get { return Net.IsThunderbolt(Description); } }
    public bool Hotspot { get { return Wireless && Ip == Net.HotspotIp; } }
    public bool HotspotClient { get { return Wireless && !Hotspot && Ip != null && Ip.StartsWith("192.168.137."); } }
    public override string ToString() { return (Thunderbolt ? "Thunderbolt" : Hotspot ? Lang.L("Wi-Fi 熱點") : Name) + Net.LinkText(Up, Speed, Wireless); }
}

// Updates come only from this repository's latest release, over HTTPS, and must match the checksum GitHub publishes.
// ponytail: the exe is unsigned, so this trusts the GitHub account; sign releases and check the signature if that ever matters.
static class Updater
{
    public class Release
    {
        public string Version, Url, Sha256;
    }

    const string LatestUrl = "https://api.github.com/repos/ikenican/LanLink/releases/latest";
    const string DownloadRoot = "https://github.com/ikenican/LanLink/releases/download/";
    const string AssetName = "LanLink.exe";
    const int MaxDownload = 64 << 20, SelfTestMs = 60000, StartMs = 30000;
    public const string StartedSignalName = "Local\\LanLink.Updated";

    // A Version with a missing part compares as older, so 0.0.3.0 would look newer than 0.0.3 without this.
    public static bool IsNewer(Version current, string latest)
    {
        Func<Version, Version> three = v => new Version(v.Major, v.Minor, Math.Max(v.Build, 0));
        return three(new Version(latest)) > three(current);
    }

    // Null unless the release names exactly the file expected, at the address expected, with a checksum.
    public static Release Parse(string json)
    {
        var root = new System.Web.Script.Serialization.JavaScriptSerializer().DeserializeObject(json) as Dictionary<string, object>;
        if (root == null || !root.ContainsKey("tag_name") || !root.ContainsKey("assets")) return null;
        string tag = root["tag_name"] as string ?? "";
        Version parsed;
        if (!tag.StartsWith("v") || !System.Version.TryParse(tag.Substring(1), out parsed)) return null;
        string url = DownloadRoot + tag + "/" + AssetName;
        foreach (var item in (root["assets"] as object[] ?? new object[0]).OfType<Dictionary<string, object>>())
        {
            if (!item.ContainsKey("browser_download_url") || !item.ContainsKey("digest")) continue;
            string digest = item["digest"] as string ?? "";
            if (item["browser_download_url"] as string != url || !digest.StartsWith("sha256:") || digest.Length != 71) continue;
            return new Release { Version = tag.Substring(1), Url = url, Sha256 = digest.Substring(7).ToLowerInvariant() };
        }
        return null;
    }

    static WebResponse Open(string url)
    {
        ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
        var request = (HttpWebRequest)WebRequest.Create(url);
        request.UserAgent = "LanLink";
        request.Timeout = 15000;
        request.ReadWriteTimeout = 30000;
        return request.GetResponse();
    }

    public static Release Latest()
    {
        using (var response = Open(LatestUrl))
        using (var reader = new StreamReader(response.GetResponseStream()))
            return Parse(reader.ReadToEnd());
    }

    public static string Sha256Of(string path)
    {
        using (var sha = System.Security.Cryptography.SHA256.Create())
        using (var f = File.OpenRead(path))
            return string.Concat(sha.ComputeHash(f).Select(b => b.ToString("x2")));
    }

    static string PendingPath(string exe) { return Path.Combine(Path.GetDirectoryName(exe), "LanLink.update.exe"); }

    // Leaves a verified copy beside the program, or throws. Running its self-test also shows whether antivirus lets it start.
    public static void Download(Release r, string exe)
    {
        string pending = PendingPath(exe);
        using (var response = Open(r.Url))
        using (var source = response.GetResponseStream())
        using (var target = File.Create(pending))
        {
            var buf = new byte[1 << 16];
            long total = 0;
            for (int n; (n = source.Read(buf, 0, buf.Length)) > 0; )
            {
                total += n;
                if (total > MaxDownload) throw new InvalidDataException("下載的檔案校驗碼不符,已取消更新。");
                target.Write(buf, 0, n);
            }
        }
        try
        {
            if (Sha256Of(pending) != r.Sha256) throw new InvalidDataException("下載的檔案校驗碼不符,已取消更新。");
            if (FileVersionInfo.GetVersionInfo(pending).ProductVersion != r.Version) throw new InvalidDataException("下載的檔案版本不符,已取消更新。");
            using (var p = Process.Start(new ProcessStartInfo(pending, "--selftest") { UseShellExecute = false, CreateNoWindow = true }))
            {
                bool done = p.WaitForExit(SelfTestMs);
                if (!done) p.Kill();
                if (!done || p.ExitCode != 0) throw new InvalidDataException("新版沒有通過自我檢查,可能被防毒軟體攔住,已取消更新。");
            }
        }
        catch
        {
            TryDelete(pending);
            throw;
        }
    }

    public static bool HasPending(string exe) { return File.Exists(PendingPath(exe)); }

    // A running exe can be renamed but not overwritten.
    public static void Swap(string exe, string pending = null)
    {
        pending = pending ?? PendingPath(exe);
        TryDelete(exe + ".old");
        File.Move(exe, exe + ".old");
        try { File.Move(pending, exe); }
        catch
        {
            File.Move(exe + ".old", exe);
            throw;
        }
    }

    // The failed copy may still be stuck running, which is why it is renamed and not deleted.
    public static void RollBack(string exe)
    {
        TryDelete(exe + ".failed");
        File.Move(exe, exe + ".failed");
        File.Move(exe + ".old", exe);
    }

    // Replaces the program and restarts it. The new copy signals as soon as it runs; without that, the old version goes back.
    public static void InstallAndRestart(string exe, bool hidden)
    {
        Swap(exe);
        using (var started = new EventWaitHandle(false, EventResetMode.AutoReset, StartedSignalName))
        {
            Process p = null;
            try
            {
                p = Process.Start(exe, "--after-update " + Process.GetCurrentProcess().Id + " " + (hidden ? "tray" : "show"));
                if (started.WaitOne(StartMs)) return;
                p.Kill();
            }
            catch (Exception) { } // Reported below.
            RollBack(exe);
            throw new InvalidOperationException("新版無法啟動,可能被防毒軟體攔住,已換回目前的版本。");
        }
    }

    public static void Cleanup(string exe)
    {
        TryDelete(exe + ".old");
        TryDelete(exe + ".failed");
        TryDelete(PendingPath(exe));
    }

    static void TryDelete(string path)
    {
        try { File.Delete(path); }
        catch (IOException) { }
        catch (UnauthorizedAccessException) { }
    }

    public static void SelfTest(Action<bool, string> check)
    {
        check(IsNewer(new Version("0.0.3.0"), "0.0.4") && IsNewer(new Version("0.0.3.0"), "0.1.0"), "update is newer");
        check(!IsNewer(new Version("0.0.3.0"), "0.0.3") && !IsNewer(new Version("0.0.3.0"), "0.0.2"), "same or older is no update");

        string hash = new string('a', 64);
        Func<string, string, string, string> json = (tag, url, digest) => "{\"tag_name\":\"" + tag + "\",\"assets\":[{\"name\":\"other.zip\"},{\"browser_download_url\":\"" + url + "\",\"digest\":\"" + digest + "\"}]}";
        var good = Parse(json("v0.0.4", DownloadRoot + "v0.0.4/LanLink.exe", "sha256:" + hash));
        check(good != null && good.Version == "0.0.4" && good.Sha256 == hash && good.Url == DownloadRoot + "v0.0.4/LanLink.exe", "release parsed");
        check(Parse(json("v0.0.4", "https://example.com/v0.0.4/LanLink.exe", "sha256:" + hash)) == null, "release from elsewhere refused");
        check(Parse(json("v0.0.4", DownloadRoot + "v0.0.3/LanLink.exe", "sha256:" + hash)) == null, "release for another tag refused");
        check(Parse(json("v0.0.4", DownloadRoot + "v0.0.4/LanLink.exe", "md5:abc")) == null, "release without a checksum refused");
        check(Parse(json("latest", DownloadRoot + "latest/LanLink.exe", "sha256:" + hash)) == null, "release without a version refused");
        check(Parse("{\"message\":\"Not Found\"}") == null && Parse("[]") == null, "not a release");

        string tmp = Path.Combine(Path.GetTempPath(), "LanLinkUpdateTest-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tmp);
        try
        {
            string exe = Path.Combine(tmp, "a.exe"), pending = PendingPath(exe);
            check(Path.GetDirectoryName(pending) == tmp, "update stays beside the program");
            File.WriteAllText(exe, "old");
            File.WriteAllText(pending, "new");
            check(HasPending(exe), "pending update found");
            Swap(exe);
            check(File.ReadAllText(exe) == "new" && File.ReadAllText(exe + ".old") == "old" && !HasPending(exe), "swap installs the new copy");
            RollBack(exe);
            check(File.ReadAllText(exe) == "old" && File.Exists(exe + ".failed") && !File.Exists(exe + ".old"), "rollback restores the old copy");
            bool threw = false;
            try { Swap(exe, Path.Combine(tmp, "missing.exe")); }
            catch (FileNotFoundException) { threw = true; }
            check(threw && File.ReadAllText(exe) == "old", "failed swap keeps the program");
            File.WriteAllText(exe + ".old", "x");
            File.WriteAllText(pending, "x");
            Cleanup(exe);
            check(File.Exists(exe) && !File.Exists(exe + ".old") && !File.Exists(exe + ".failed") && !HasPending(exe), "cleanup keeps only the program");
            File.WriteAllBytes(pending, Encoding.ASCII.GetBytes("abc"));
            check(Sha256Of(pending) == "ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad", "sha-256 check value");
        }
        finally { Directory.Delete(tmp, true); }
    }
}

// QR Code, byte mode, low error correction. Stops at version 6 (134 bytes), which fits an address and any normal Wi-Fi login;
// version 7 and up also need version bits. Encode returns null for longer text.
static class Qr
{
    static readonly int[] DataWords = { 19, 34, 55, 80, 108, 136 };
    static readonly int[] EcWords = { 7, 10, 15, 20, 26, 18 };
    static readonly int[] Blocks = { 1, 1, 1, 1, 1, 2 };
    static readonly int[] Alignment = { 0, 18, 22, 26, 30, 34 };

    static int Multiply(int a, int b)
    {
        int r = 0;
        for (int i = 7; i >= 0; i--)
        {
            r = (r << 1) ^ ((r >> 7) * 0x11D);
            r ^= ((b >> i) & 1) * a;
        }
        return r;
    }

    public static byte[] ReedSolomon(byte[] data, int count)
    {
        var divisor = new int[count];
        divisor[count - 1] = 1;
        int root = 1;
        for (int i = 0; i < count; i++)
        {
            for (int j = 0; j < count; j++)
            {
                divisor[j] = Multiply(divisor[j], root);
                if (j + 1 < count) divisor[j] ^= divisor[j + 1];
            }
            root = Multiply(root, 2);
        }
        var result = new byte[count];
        foreach (byte b in data)
        {
            int factor = b ^ result[0];
            Array.Copy(result, 1, result, 0, count - 1);
            result[count - 1] = 0;
            for (int i = 0; i < count; i++) result[i] ^= (byte)Multiply(divisor[i], factor);
        }
        return result;
    }

    public static int FormatBits(int mask)
    {
        int data = 1 << 3 | mask; // 01 is low error correction.
        int rem = data;
        for (int i = 0; i < 10; i++) rem = (rem << 1) ^ ((rem >> 9) * 0x537);
        return (data << 10 | rem) ^ 0x5412;
    }

    static byte[] Codewords(byte[] text, int version)
    {
        var bits = new List<bool>();
        Action<int, int> add = (value, count) => { for (int i = count - 1; i >= 0; i--) bits.Add(((value >> i) & 1) != 0); };
        add(4, 4);
        add(text.Length, 8);
        foreach (byte b in text) add(b, 8);
        int capacity = DataWords[version] * 8;
        add(0, Math.Min(4, capacity - bits.Count));
        add(0, (8 - bits.Count % 8) % 8);
        var data = new byte[DataWords[version]];
        for (int i = 0; i < data.Length; i++)
        {
            if (i * 8 < bits.Count) for (int k = 0; k < 8; k++) data[i] = (byte)(data[i] << 1 | (bits[i * 8 + k] ? 1 : 0));
            else data[i] = (byte)((i - bits.Count / 8) % 2 == 0 ? 0xEC : 0x11);
        }

        // Equal block sizes in these versions, so interleaving is just taking turns.
        int blocks = Blocks[version], size = data.Length / blocks, ec = EcWords[version];
        var codes = Enumerable.Range(0, blocks).Select(b => ReedSolomon(data.Skip(b * size).Take(size).ToArray(), ec)).ToList();
        var result = new List<byte>();
        for (int i = 0; i < size; i++) for (int b = 0; b < blocks; b++) result.Add(data[b * size + i]);
        for (int i = 0; i < ec; i++) for (int b = 0; b < blocks; b++) result.Add(codes[b][i]);
        return result.ToArray();
    }

    static void DrawFormat(bool[,] m, bool[,] fixedCell, int mask)
    {
        int bits = FormatBits(mask), n = m.GetLength(0);
        Action<int, int, int> put = (x, y, bit) =>
        {
            m[y, x] = ((bits >> bit) & 1) != 0;
            fixedCell[y, x] = true;
        };
        for (int i = 0; i <= 5; i++) put(8, i, i);
        put(8, 7, 6);
        put(8, 8, 7);
        put(7, 8, 8);
        for (int i = 9; i < 15; i++) put(14 - i, 8, i);
        for (int i = 0; i < 8; i++) put(n - 1 - i, 8, i);
        for (int i = 8; i < 15; i++) put(8, n - 15 + i, i);
    }

    static bool MaskHit(int mask, int x, int y)
    {
        switch (mask)
        {
            case 0: return (x + y) % 2 == 0;
            case 1: return y % 2 == 0;
            case 2: return x % 3 == 0;
            case 3: return (x + y) % 3 == 0;
            case 4: return (x / 3 + y / 2) % 2 == 0;
            case 5: return x * y % 2 + x * y % 3 == 0;
            case 6: return (x * y % 2 + x * y % 3) % 2 == 0;
            default: return ((x + y) % 2 + x * y % 3) % 2 == 0;
        }
    }

    // ponytail: scores long runs and dark balance only, not the standard's 2x2 and finder-lookalike rules. Any mask is valid, this only helps scanning.
    static int Penalty(bool[,] m)
    {
        int n = m.GetLength(0), score = 0, dark = 0;
        for (int a = 0; a < n; a++)
        {
            int rowRun = 1, colRun = 1;
            for (int b = 0; b < n; b++)
            {
                if (m[a, b]) dark++;
                if (b == 0) continue;
                rowRun = m[a, b] == m[a, b - 1] ? rowRun + 1 : 1;
                colRun = m[b, a] == m[b - 1, a] ? colRun + 1 : 1;
                if (rowRun == 5 || colRun == 5) score += 3;
                else
                {
                    if (rowRun > 5) score++;
                    if (colRun > 5) score++;
                }
            }
        }
        return score + Math.Abs(dark * 20 - n * n * 10) / (n * n) * 10;
    }

    public static bool[,] Encode(string text)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(text);
        int version = Enumerable.Range(0, DataWords.Length).Where(i => bytes.Length + 2 <= DataWords[i]).DefaultIfEmpty(-1).First();
        if (version < 0) return null;

        int n = 21 + 4 * version;
        var m = new bool[n, n];
        var fixedCell = new bool[n, n];
        Action<int, int, bool> set = (x, y, dark) =>
        {
            m[y, x] = dark;
            fixedCell[y, x] = true;
        };
        foreach (var corner in new[] { new[] { 0, 0 }, new[] { n - 7, 0 }, new[] { 0, n - 7 } })
            for (int dy = -1; dy <= 7; dy++)
                for (int dx = -1; dx <= 7; dx++)
                {
                    int x = corner[0] + dx, y = corner[1] + dy;
                    if (x < 0 || y < 0 || x >= n || y >= n) continue;
                    int ring = Math.Max(Math.Abs(dx - 3), Math.Abs(dy - 3));
                    set(x, y, ring != 2 && ring != 4);
                }
        for (int i = 8; i < n - 8; i++)
        {
            set(i, 6, i % 2 == 0);
            set(6, i, i % 2 == 0);
        }
        if (version > 0)
            for (int dy = -2; dy <= 2; dy++)
                for (int dx = -2; dx <= 2; dx++)
                    set(Alignment[version] + dx, Alignment[version] + dy, Math.Max(Math.Abs(dx), Math.Abs(dy)) != 1);
        DrawFormat(m, fixedCell, 0); // Reserves the cells; each mask below draws its own.
        set(8, n - 8, true);

        // Two columns at a time from the right, snaking up and down, stepping over the vertical timing line.
        byte[] words = Codewords(bytes, version);
        int bit = 0;
        for (int right = n - 1; right >= 1; right -= 2)
        {
            if (right == 6) right = 5;
            bool upward = ((right + 1) & 2) == 0;
            for (int step = 0; step < n; step++)
                for (int j = 0; j < 2; j++)
                {
                    int x = right - j, y = upward ? n - 1 - step : step;
                    if (fixedCell[y, x] || bit >= words.Length * 8) continue;
                    m[y, x] = ((words[bit >> 3] >> (7 - (bit & 7))) & 1) != 0;
                    bit++;
                }
        }

        bool[,] best = null;
        int bestScore = int.MaxValue;
        for (int mask = 0; mask < 8; mask++)
        {
            var candidate = (bool[,])m.Clone();
            for (int y = 0; y < n; y++)
                for (int x = 0; x < n; x++)
                    if (!fixedCell[y, x] && MaskHit(mask, x, y)) candidate[y, x] = !candidate[y, x];
            DrawFormat(candidate, fixedCell, mask);
            int score = Penalty(candidate);
            if (score < bestScore)
            {
                bestScore = score;
                best = candidate;
            }
        }
        return best;
    }

    public static void SelfTest(Action<bool, string> check)
    {
        // The worked example in the QR standard: version 1-M, "01234567".
        var data = new byte[] { 0x10, 0x20, 0x0C, 0x56, 0x61, 0x80, 0xEC, 0x11, 0xEC, 0x11, 0xEC, 0x11, 0xEC, 0x11, 0xEC, 0x11 };
        check(ReedSolomon(data, 10).SequenceEqual(new byte[] { 0xA5, 0x24, 0xD4, 0xC1, 0xED, 0x36, 0xC7, 0x87, 0x2C, 0x55 }), "qr error correction");
        check(FormatBits(0) == 0x77C4 && FormatBits(7) == 0x6976, "qr format bits");
        check(Multiply(0x80, 2) == 0x1D, "qr field multiply");

        var small = Encode("hi");
        check(small.GetLength(0) == 21, "qr smallest version");
        check(Encode(new string('x', 17)).GetLength(0) == 21 && Encode(new string('x', 18)).GetLength(0) == 25, "qr version chosen by length");
        check(Encode(new string('x', 134)).GetLength(0) == 41 && Encode(new string('x', 135)) == null, "qr longest text");
        check(Encode("中").GetLength(0) == 21, "qr utf-8 text");
        foreach (var length in new[] { 2, 18, 40, 70, 100, 134 })
        {
            var m = Encode(new string('x', length));
            int n = m.GetLength(0);
            bool ok = true;
            foreach (var c in new[] { new[] { 0, 0 }, new[] { n - 7, 0 }, new[] { 0, n - 7 } })
                for (int i = 0; i < 7; i++)
                    ok &= m[c[1], c[0] + i] && m[c[1] + 6, c[0] + i] && m[c[1] + i, c[0]] && m[c[1] + i, c[0] + 6];
            ok &= m[3, 3] && !m[1, 1] && !m[7, 7];
            for (int i = 8; i < n - 8; i++) ok &= m[6, i] == (i % 2 == 0) && m[i, 6] == (i % 2 == 0);
            ok &= m[n - 8, 8];
            // Both copies of the format bits must agree, or a scanner can't start.
            for (int i = 0; i <= 5; i++) ok &= m[i, 8] == m[8, n - 1 - i];
            check(ok, "qr fixed patterns at length " + length);
        }
    }
}

// A ZIP with no compression, written while sending: nothing is held in memory or on disk, and the size is known up front.
// csc's default references don't include System.IO.Compression.
static class Zip
{
    const ushort Flags = 0x0808; // Sizes and CRC follow the data, names are UTF-8.
    const ushort Version = 20;
    const int LocalHeader = 30, Descriptor = 16, CentralHeader = 46, End = 22;
    static readonly uint[] Table = Enumerable.Range(0, 256).Select(n =>
    {
        uint c = (uint)n;
        for (int k = 0; k < 8; k++) c = (c & 1) != 0 ? 0xEDB88320 ^ (c >> 1) : c >> 1;
        return c;
    }).ToArray();

    public static uint Crc(uint crc, byte[] data, int count)
    {
        for (int i = 0; i < count; i++) crc = Table[(crc ^ data[i]) & 0xFF] ^ (crc >> 8);
        return crc;
    }

    static byte[] Name(TransferItem f) { return Encoding.UTF8.GetBytes(f.Rel.Replace('\\', '/')); }

    public static long Size(IEnumerable<TransferItem> files)
    {
        return files.Sum(f => LocalHeader + Descriptor + CentralHeader + 2L * Name(f).Length + f.Length) + End;
    }

    // ponytail: no ZIP64, which is what caps a folder at 4 GB. Larger ones are offered file by file; add ZIP64 if that gets tedious.
    public static bool Fits(List<TransferItem> files) { return files.Count < 65535 && Size(files) < uint.MaxValue; }

    static uint DosTime(DateTime t)
    {
        if (t.Year < 1980) return 0x00210000; // 1980-01-01, the earliest a ZIP can store.
        return (uint)((t.Year - 1980) << 25 | t.Month << 21 | t.Day << 16 | t.Hour << 11 | t.Minute << 5 | t.Second / 2);
    }

    public static void Write(Stream target, List<TransferItem> files)
    {
        var o = new BinaryWriter(new BufferedStream(target, 1 << 16));
        var directory = new MemoryStream();
        var d = new BinaryWriter(directory);
        uint offset = 0;
        foreach (var f in files)
        {
            byte[] name = Name(f);
            uint time = DosTime(File.GetLastWriteTime(f.Full));
            o.Write(0x04034b50u); o.Write(Version); o.Write(Flags); o.Write((ushort)0); o.Write(time);
            o.Write(0u); o.Write(0u); o.Write(0u); o.Write((ushort)name.Length); o.Write((ushort)0); o.Write(name);
            o.Flush();
            uint crc = ~WebShare.Copy(f.Full, f.Length, o.BaseStream, true);
            uint size = (uint)f.Length;
            o.Write(0x08074b50u); o.Write(crc); o.Write(size); o.Write(size);

            d.Write(0x02014b50u); d.Write(Version); d.Write(Version); d.Write(Flags); d.Write((ushort)0); d.Write(time);
            d.Write(crc); d.Write(size); d.Write(size); d.Write((ushort)name.Length);
            d.Write((ushort)0); d.Write((ushort)0); d.Write((ushort)0); d.Write((ushort)0); d.Write(0u); d.Write(offset); d.Write(name);
            offset += (uint)(LocalHeader + name.Length + Descriptor) + size;
        }
        d.Flush();
        o.Write(directory.ToArray());
        o.Write(0x06054b50u); o.Write((ushort)0); o.Write((ushort)0); o.Write((ushort)files.Count); o.Write((ushort)files.Count);
        o.Write((uint)directory.Length); o.Write(offset); o.Write((ushort)0);
        o.Flush();
    }
}

// Lets a PC with no tool and no setup download with its browser. Requests only name a number from the list, never a path.
class WebShare
{
    class Entry
    {
        public string Name;
        public int First, Count;
        public bool Folder;
    }

    class Content
    {
        public List<TransferItem> Files = new List<TransferItem>();
        public List<Entry> Entries = new List<Entry>();
    }

    const int MaxRequest = 8192, BufferSize = 1 << 20;
    readonly TcpListener listener;
    readonly Func<string, bool> allowed;
    readonly Action<string> downloading, received;
    readonly Func<string> inbox;
    readonly object changing = new object();
    volatile Content content = new Content();

    public int Port { get { return ((IPEndPoint)listener.LocalEndpoint).Port; } }

    // Uploads go to inbox(); received gets each saved file's full path.
    public WebShare(IPAddress address, int port, Func<string, bool> allowed, Action<string> downloading, Func<string> inbox, Action<string> received)
    {
        this.allowed = allowed;
        this.downloading = downloading;
        this.inbox = inbox;
        this.received = received;
        listener = new TcpListener(address, port);
        listener.Start();
        new Thread(Accept) { IsBackground = true }.Start();
    }

    public void Stop() { listener.Stop(); }

    public void Clear() { content = new Content(); }

    public void Add(IEnumerable<string> paths)
    {
        lock (changing)
        {
            var next = new Content { Files = content.Files.ToList(), Entries = content.Entries.ToList() };
            foreach (var path in paths)
            {
                var found = Transfer.Collect(new[] { path }).Where(i => i.Length >= 0).ToList();
                bool folder = Directory.Exists(path);
                next.Entries.Add(new Entry { Name = Path.GetFileName(path.TrimEnd('\\')), First = next.Files.Count, Count = found.Count, Folder = folder });
                next.Files.AddRange(found);
            }
            content = next;
        }
    }

    public string Summary()
    {
        var c = content;
        if (c.Entries.Count == 0) return Lang.L("還沒有檔案,把檔案或資料夾拖到這裡。");
        return string.Join("\n", c.Entries.Select(e => e.Folder ? string.Format(Lang.L("資料夾:{0}({1} 個檔案)"), e.Name, e.Count) : e.Name));
    }

    void Accept()
    {
        while (true)
        {
            TcpClient client;
            try { client = listener.AcceptTcpClient(); }
            catch (SocketException) { return; } // Stopped.
            catch (ObjectDisposedException) { return; }
            ThreadPool.QueueUserWorkItem(_ => Serve(client));
        }
    }

    void Serve(TcpClient client)
    {
        // Closing the connection is how a failed download is reported, and nothing here may take the app down.
        try
        {
            using (client)
            {
                if (!allowed(((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString())) return;
                client.ReceiveTimeout = 15000;
                client.SendTimeout = 60000;
                var s = client.GetStream();
                string head = ReadHead(s);
                long length;
                string name = ParseUpload(head, out length);
                if (name != null) Receive(s, name, length);
                else Respond(s, ParseRequest(head));
            }
        }
        catch (Exception) { }
    }

    static string ReadHead(Stream s)
    {
        var head = new StringBuilder();
        var one = new byte[1];
        while (head.Length < MaxRequest && s.Read(one, 0, 1) == 1)
        {
            head.Append((char)one[0]);
            if (head.Length >= 4 && head.ToString(head.Length - 4, 4) == "\r\n\r\n") break;
        }
        return head.ToString();
    }

    // The path of a GET request, or null for anything else.
    public static string ParseRequest(string head)
    {
        var parts = head.Split(new[] { "\r\n" }, StringSplitOptions.None)[0].Split(' ');
        if (parts.Length != 3 || parts[0] != "GET" || !parts[2].StartsWith("HTTP/")) return null;
        return parts[1].Split('?')[0];
    }

    // The safe file name of a "POST /u?n=<name>" upload with a Content-Length, or null for anything else.
    public static string ParseUpload(string head, out long length)
    {
        length = -1;
        var lines = head.Split(new[] { "\r\n" }, StringSplitOptions.None);
        var parts = lines[0].Split(' ');
        if (parts.Length != 3 || parts[0] != "POST" || !parts[2].StartsWith("HTTP/") || !parts[1].StartsWith("/u?n=")) return null;
        foreach (var l in lines)
            if (l.StartsWith("Content-Length:", StringComparison.OrdinalIgnoreCase) && !long.TryParse(l.Substring(15).Trim(), out length)) length = -1;
        // Only a bare name: the page sends files one by one, and nothing may land outside the inbox.
        string name = Net.SafeRelativePath(Uri.UnescapeDataString(parts[1].Substring(5)));
        return length >= 0 && name != null && !name.Contains("\\") ? name : null;
    }

    // Writes to a part file first, so a broken upload never leaves a file that looks complete.
    void Receive(Stream s, string name, long length)
    {
        string root = inbox();
        Directory.CreateDirectory(root);
        string target = Net.UniquePath(Path.Combine(root, name), p => File.Exists(p) || Directory.Exists(p));
        string part = target + Transfer.PartSuffix;
        try
        {
            var buf = new byte[BufferSize];
            using (var f = new FileStream(part, FileMode.Create, FileAccess.Write))
                for (long left = length; left > 0; )
                {
                    int n = s.Read(buf, 0, (int)Math.Min(left, buf.Length));
                    if (n == 0) throw new EndOfStreamException();
                    f.Write(buf, 0, n);
                    left -= n;
                }
            File.Move(part, target);
        }
        catch
        {
            File.Delete(part);
            throw;
        }
        received(target);
        byte[] ok = Encoding.ASCII.GetBytes("OK");
        Head(s, "200 OK", "text/plain", ok.Length, null);
        s.Write(ok, 0, ok.Length);
    }

    public static bool TryIndex(string path, string prefix, int count, out int index)
    {
        index = -1;
        if (path == null || !path.StartsWith(prefix)) return false;
        string number = path.Substring(prefix.Length);
        return number.Length > 0 && number.Length < 10 && number.All(char.IsDigit) && int.TryParse(number, out index) && index < count;
    }

    void Respond(Stream s, string path)
    {
        var c = content;
        int n;
        if (path == "/")
        {
            byte[] page = Encoding.UTF8.GetBytes(Page(c));
            Head(s, "200 OK", "text/html; charset=utf-8", page.Length, null);
            s.Write(page, 0, page.Length);
        }
        else if (path == "/LanLink.exe")
        {
            SendFile(s, Sys.ProgramPath, "LanLink.exe", new FileInfo(Sys.ProgramPath).Length);
        }
        else if (TryIndex(path, "/f/", c.Files.Count, out n))
        {
            SendFile(s, c.Files[n].Full, Path.GetFileName(c.Files[n].Rel), c.Files[n].Length);
        }
        else if (TryIndex(path, "/z/", c.Entries.Count, out n) && c.Entries[n].Folder && Zip.Fits(Range(c, c.Entries[n])))
        {
            var files = Range(c, c.Entries[n]);
            string name = c.Entries[n].Name + ".zip";
            downloading(name);
            Head(s, "200 OK", "application/zip", Zip.Size(files), name);
            Zip.Write(s, files);
        }
        else
        {
            byte[] missing = Encoding.ASCII.GetBytes("Not found");
            Head(s, "404 Not Found", "text/plain", missing.Length, null);
            s.Write(missing, 0, missing.Length);
        }
    }

    static List<TransferItem> Range(Content c, Entry e) { return c.Files.GetRange(e.First, e.Count); }

    void SendFile(Stream s, string full, string name, long length)
    {
        downloading(name);
        Head(s, "200 OK", "application/octet-stream", length, name);
        Copy(full, length, s, false);
    }

    static void Head(Stream s, string status, string type, long length, string downloadName)
    {
        string head = "HTTP/1.1 " + status + "\r\nContent-Type: " + type + "\r\nContent-Length: " + length
            + "\r\nConnection: close\r\nCache-Control: no-store\r\nX-Content-Type-Options: nosniff\r\n"
            + (downloadName == null ? "" : "Content-Disposition: attachment; filename*=UTF-8''" + Uri.EscapeDataString(downloadName) + "\r\n")
            + "\r\n";
        byte[] bytes = Encoding.ASCII.GetBytes(head);
        s.Write(bytes, 0, bytes.Length);
    }

    // Sends exactly length bytes, so the size already announced stays true. A file that shrank fails the download.
    // Returns the running CRC when asked for: ponytail: this table CRC tops out near 500 MB/s, so a ZIP is slower than a single file; use slicing-by-8 if it matters.
    public static uint Copy(string full, long length, Stream target, bool withCrc)
    {
        uint crc = 0xFFFFFFFF;
        var buf = new byte[BufferSize];
        using (var f = new FileStream(full, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete, BufferSize, FileOptions.SequentialScan))
        {
            for (long left = length; left > 0; )
            {
                int n = f.Read(buf, 0, (int)Math.Min(left, buf.Length));
                if (n == 0) throw new EndOfStreamException();
                if (withCrc) crc = Zip.Crc(crc, buf, n);
                target.Write(buf, 0, n);
                left -= n;
            }
        }
        return crc;
    }

    static string SizeText(long bytes)
    {
        var inv = System.Globalization.CultureInfo.InvariantCulture;
        return bytes >= 1000000000 ? (bytes / 1e9).ToString("0.##", inv) + " GB" : (bytes / 1e6).ToString("0.##", inv) + " MB";
    }

    static string Link(string href, string text, long bytes)
    {
        return "<a href=\"" + href + "\">" + WebUtility.HtmlEncode(text) + " <small>" + SizeText(bytes) + "</small></a>";
    }

    static string Page(Content c)
    {
        string title = WebUtility.HtmlEncode(Lang.L("LAN 互連 檔案下載"));
        var b = new StringBuilder("<!doctype html><html><head><meta charset=\"utf-8\"><meta name=\"viewport\" content=\"width=device-width\"><title>" + title + "</title>");
        b.Append("<style>:root{color-scheme:light dark}body{font:16px system-ui,sans-serif;max-width:640px;margin:40px auto;padding:0 16px}");
        b.Append("a{display:block;padding:12px 14px;margin:8px 0;border:1px solid #8886;border-radius:10px;text-decoration:none}small{opacity:.7}hr{border:0;border-top:1px solid #8886;margin:24px 0}</style></head><body>");
        b.Append("<h2>" + title + "</h2>");
        if (c.Entries.Count == 0) b.Append("<p>" + WebUtility.HtmlEncode(Lang.L("對方還沒有選擇檔案,這個頁面會自動更新。")) + "</p>");
        for (int i = 0; i < c.Entries.Count; i++)
        {
            var e = c.Entries[i];
            var files = Range(c, e);
            if (!e.Folder)
            {
                b.Append(Link("/f/" + e.First, e.Name, files[0].Length));
            }
            else if (Zip.Fits(files))
            {
                b.Append(Link("/z/" + i, string.Format(Lang.L("下載資料夾 {0}.zip"), e.Name), Zip.Size(files)));
            }
            else
            {
                b.Append("<p>" + WebUtility.HtmlEncode(string.Format(Lang.L("這個資料夾太大,無法打包,請逐檔下載:{0}"), e.Name)) + "</p>");
                for (int k = 0; k < e.Count; k++) b.Append(Link("/f/" + (e.First + k), files[k].Rel, files[k].Length));
            }
        }
        b.Append("<hr><h3>" + WebUtility.HtmlEncode(Lang.L("上傳檔案給這台")) + "</h3><input type=\"file\" id=\"u\" multiple>");
        b.Append("<p id=\"s\" data-done=\"" + WebUtility.HtmlEncode(Lang.L("上傳完成 {0} 個,失敗 {1} 個。")) + "\"></p>");
        // One file at a time, and the empty page's refresh waits, so nothing cuts an upload short.
        b.Append("<script>var busy=0,s=document.getElementById('s');");
        b.Append("document.getElementById('u').onchange=function(){var q=[].slice.call(this.files),i=0,ok=0;this.value='';busy=1;");
        b.Append("(function next(){if(i==q.length){busy=0;s.textContent=s.dataset.done.replace('{0}',ok).replace('{1}',q.length-ok);return}");
        b.Append("var f=q[i++],x=new XMLHttpRequest();x.open('POST','/u?n='+encodeURIComponent(f.name));");
        b.Append("x.upload.onprogress=function(e){if(e.total)s.textContent=f.name+' '+Math.floor(e.loaded*100/e.total)+'%'};");
        b.Append("x.onload=function(){if(x.status==200)ok++;next()};x.onerror=next;x.send(f)})()};");
        if (c.Entries.Count == 0) b.Append("setInterval(function(){if(!busy)location.reload()},3000);");
        b.Append("</script>");
        b.Append("<hr>" + Link("/LanLink.exe", Lang.L("下載 LAN 互連工具"), new FileInfo(Sys.ProgramPath).Length));
        b.Append("</body></html>");
        return b.ToString();
    }

    public static void SelfTest(Action<bool, string> check)
    {
        check(ParseRequest("GET /f/1?x=2 HTTP/1.1\r\nHost: a\r\n\r\n") == "/f/1", "web request path");
        check(ParseRequest("POST /f/1 HTTP/1.1\r\n\r\n") == null && ParseRequest("") == null && ParseRequest("GET /a b HTTP/1.1\r\n\r\n") == null, "web request refused");
        long size;
        check(ParseUpload("POST /u?n=%E6%AA%94%20a.txt HTTP/1.1\r\ncontent-length: 5\r\n\r\n", out size) == "檔 a.txt" && size == 5, "upload request");
        foreach (var bad in new[] { "POST /u?n=a.txt HTTP/1.1\r\n\r\n", "POST /u?n=a.txt HTTP/1.1\r\nContent-Length: -1\r\n\r\n", "POST /u?n=a.txt HTTP/1.1\r\nContent-Length: x\r\n\r\n",
            "POST /u?n=..%5Ca.txt HTTP/1.1\r\nContent-Length: 1\r\n\r\n", "POST /u?n=d%2Fa.txt HTTP/1.1\r\nContent-Length: 1\r\n\r\n", "POST /u?n=a%3Ab HTTP/1.1\r\nContent-Length: 1\r\n\r\n",
            "POST /u?n=CON HTTP/1.1\r\nContent-Length: 1\r\n\r\n", "POST /u?n= HTTP/1.1\r\nContent-Length: 1\r\n\r\n", "GET /u?n=a.txt HTTP/1.1\r\nContent-Length: 1\r\n\r\n" })
            check(ParseUpload(bad, out size) == null, "upload refused: " + bad.Split('\r')[0]);
        int n;
        check(TryIndex("/f/2", "/f/", 3, out n) && n == 2, "web index");
        check(!TryIndex("/f/3", "/f/", 3, out n) && !TryIndex("/f/-1", "/f/", 3, out n) && !TryIndex("/f/..\\x", "/f/", 3, out n) && !TryIndex("/f/", "/f/", 3, out n) && !TryIndex("/f/99999999999", "/f/", 3, out n), "web index refused");
        byte[] standard = Encoding.ASCII.GetBytes("123456789");
        check(~Zip.Crc(0xFFFFFFFF, standard, standard.Length) == 0xCBF43926, "crc32 check value");

        string tmp = Path.Combine(Path.GetTempPath(), "LanLinkWebTest-" + Guid.NewGuid().ToString("N"));
        string src = Path.Combine(tmp, "src"), dst = Path.Combine(src, "資料夾", "子");
        Directory.CreateDirectory(dst);
        string secret = Path.Combine(tmp, "secret.txt");
        File.WriteAllText(secret, "secret");
        string single = Path.Combine(src, "a&b 檔案.bin"), folder = Path.Combine(src, "資料夾");
        var big = new byte[BufferSize + 17];
        new Random(7).NextBytes(big);
        File.WriteAllBytes(single, big);
        File.WriteAllText(Path.Combine(folder, "one.txt"), "one");
        File.WriteAllBytes(Path.Combine(dst, "two.bin"), big);
        File.WriteAllBytes(Path.Combine(dst, "empty.txt"), new byte[0]);
        var started = new List<string>();
        string up = Path.Combine(tmp, "up");
        var saved = new List<string>();
        var web = new WebShare(IPAddress.Loopback, 0, a => true, name => { lock (started) started.Add(name); }, () => up, p => { lock (saved) saved.Add(p); });
        try
        {
            string root = "http://127.0.0.1:" + web.Port;
            var http = new WebClient { Proxy = null, Encoding = Encoding.UTF8 };
            check(http.DownloadString(root + "/").Contains("reload"), "empty page refreshes");
            web.Add(new[] { single, folder });
            string page = http.DownloadString(root + "/");
            check(page.Contains("a&amp;b 檔案.bin") && page.Contains("/z/1") && !page.Contains("reload") && !page.Contains(src) && page.Contains("/u?n="), "page lists items without paths");

            string upUrl = root + "/u?n=" + Uri.EscapeDataString("上傳 a.bin");
            check(Encoding.ASCII.GetString(http.UploadData(upUrl, big)) == "OK", "upload accepted");
            http.UploadData(upUrl, new byte[0]);
            check(File.ReadAllBytes(Path.Combine(up, "上傳 a.bin")).SequenceEqual(big) && new FileInfo(Path.Combine(up, "上傳 a (1).bin")).Length == 0, "upload saved without overwriting");
            check(Directory.GetFiles(up).Length == 2, "upload leaves no part files");
            lock (saved) check(saved.Count == 2 && saved[1] == Path.Combine(up, "上傳 a (1).bin"), "uploads reported");
            bool upBlocked = false;
            try { http.UploadData(root + "/u?n=..%5Csecret2.txt", new byte[] { 1 }); }
            catch (WebException) { upBlocked = true; }
            check(upBlocked && !File.Exists(Path.Combine(tmp, "secret2.txt")), "upload refuses escaping names");

            check(http.DownloadData(root + "/f/0").SequenceEqual(big), "single file download");
            check((http.ResponseHeaders["Content-Disposition"] ?? "").Contains("UTF-8''a%26b"), "download name");
            check(http.DownloadData(root + "/LanLink.exe").Length == new FileInfo(Sys.ProgramPath).Length, "tool download");

            byte[] zip = http.DownloadData(root + "/z/1");
            check(zip.Length.ToString() == http.ResponseHeaders["Content-Length"], "zip size announced");
            // A real ZIP reader, so a format mistake fails here and not on the other PC.
            var reader = Assembly.Load("System.IO.Compression, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089").GetType("System.IO.Compression.ZipArchive");
            dynamic archive = Activator.CreateInstance(reader, new object[] { new MemoryStream(zip) });
            var names = new Dictionary<string, byte[]>();
            foreach (dynamic entry in archive.Entries)
            {
                var copy = new MemoryStream();
                using (Stream part = entry.Open()) part.CopyTo(copy);
                names[(string)entry.FullName] = copy.ToArray();
            }
            check(names.Count == 3, "zip entry count");
            check(names.ContainsKey("資料夾/one.txt") && Encoding.ASCII.GetString(names["資料夾/one.txt"]) == "one", "zip text entry");
            check(names.ContainsKey("資料夾/子/two.bin") && names["資料夾/子/two.bin"].SequenceEqual(big), "zip binary entry");
            check(names.ContainsKey("資料夾/子/empty.txt") && names["資料夾/子/empty.txt"].Length == 0, "zip empty entry");
            lock (started) check(started.Contains("資料夾.zip") && started.Contains("LanLink.exe"), "downloads reported");

            foreach (var refused in new[] { "/f/99", "/z/0", "/f/..%5c..%5csecret.txt", "/secret.txt" })
            {
                bool blocked = false;
                try { http.DownloadData(root + refused); }
                catch (WebException) { blocked = true; }
                check(blocked, "web refuses " + refused);
            }
            web.Clear();
            check(http.DownloadString(root + "/").Contains("reload"), "cleared list");
        }
        finally
        {
            web.Stop();
            Directory.Delete(tmp, true);
        }
    }
}

// Everything that touches the OS.
static class Sys
{
    [DllImport("kernel32")] static extern int GetOEMCP();
    [DllImport("shell32", CharSet = CharSet.Unicode)] static extern int SHGetKnownFolderPath(ref Guid id, uint flags, IntPtr token, out string path);
    [DllImport("iphlpapi")] static extern int GetIpNetTable(IntPtr table, ref int size, bool order);
    const int ErrorCancelled = 1223;

    public static readonly string DataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LanLink");
    static readonly string InboxSettingPath = Path.Combine(DataDir, "inbox.txt");
    static volatile string inbox;

    // Asks Windows, because the Downloads folder can be moved to another drive.
    public static string DownloadsFolder()
    {
        var id = new Guid("374DE290-123F-4565-9164-39C4925E467B"); // FOLDERID_Downloads
        string path;
        return SHGetKnownFolderPath(ref id, 0, IntPtr.Zero, out path) == 0 ? path
            : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
    }

    // The chosen receive folder, or the Windows Downloads folder when none is saved.
    public static string Inbox
    {
        get
        {
            if (inbox != null) return inbox;
            string saved = "";
            try { saved = File.ReadAllText(InboxSettingPath).Trim(); }
            catch (IOException) { }
            inbox = Path.IsPathRooted(saved) ? saved : DownloadsFolder();
            return inbox;
        }
        set
        {
            Directory.CreateDirectory(DataDir);
            File.WriteAllText(InboxSettingPath, value);
            inbox = value;
        }
    }

    public static string ProgramPath { get { return Assembly.GetExecutingAssembly().Location; } }

    const string RunKey = @"Software\Microsoft\Windows\CurrentVersion\Run";

    public static bool AutoStart
    {
        get
        {
            using (var k = Registry.CurrentUser.OpenSubKey(RunKey))
                return k != null && k.GetValue("LanLink") != null;
        }
        set
        {
            using (var k = Registry.CurrentUser.CreateSubKey(RunKey))
            {
                if (value) k.SetValue("LanLink", "\"" + ProgramPath + "\" --tray");
                else k.DeleteValue("LanLink", false);
            }
        }
    }

    public static bool QuitOnClose
    {
        get { return File.Exists(QuitOnClosePath); }
        set
        {
            Directory.CreateDirectory(DataDir);
            if (value) File.WriteAllText(QuitOnClosePath, "1");
            else File.Delete(QuitOnClosePath);
        }
    }

    // On unless switched off, so a missing setting still asks.
    public static bool AskRestore
    {
        get { return !File.Exists(NoAskRestorePath); }
        set
        {
            Directory.CreateDirectory(DataDir);
            if (value) File.Delete(NoAskRestorePath);
            else File.WriteAllText(NoAskRestorePath, "1");
        }
    }

    public static bool AutoRestore
    {
        get { return File.Exists(AutoRestorePath); }
        set
        {
            Directory.CreateDirectory(DataDir);
            if (value) File.WriteAllText(AutoRestorePath, "1");
            else File.Delete(AutoRestorePath);
        }
    }

    public static bool AutoUpdate
    {
        get { return !File.Exists(NoAutoUpdatePath); }
        set
        {
            Directory.CreateDirectory(DataDir);
            if (value) File.Delete(NoAutoUpdatePath);
            else File.WriteAllText(NoAutoUpdatePath, "1");
        }
    }

    public static bool WindowsUi
    {
        get { return File.Exists(WindowsUiPath); }
        set
        {
            Directory.CreateDirectory(DataDir);
            if (value) File.WriteAllText(WindowsUiPath, "1");
            else File.Delete(WindowsUiPath);
        }
    }

    public static void ResetInbox()
    {
        if (File.Exists(InboxSettingPath)) File.Delete(InboxSettingPath);
        inbox = null;
    }

    static readonly string SessionPath = Path.Combine(DataDir, "session.txt");
    public static readonly string ThemePath = Path.Combine(DataDir, "theme.txt");
    public static readonly string SendToPath = Path.Combine(DataDir, "sendto.txt");
    static readonly string QuitOnClosePath = Path.Combine(DataDir, "quit-on-close.txt");
    static readonly string WindowsUiPath = Path.Combine(DataDir, "windows-ui.txt");
    static readonly string NoAutoUpdatePath = Path.Combine(DataDir, "no-auto-update.txt");
    static readonly string NoAskRestorePath = Path.Combine(DataDir, "no-ask-restore.txt");
    static readonly string AutoRestorePath = Path.Combine(DataDir, "auto-restore.txt");
    static readonly string RdpBackupPath = Path.Combine(DataDir, "rdp.txt");

    static string BackupPath(string id) { return Path.Combine(DataDir, id + ".txt"); }

    public static List<Adapter> Adapters()
    {
        // ponytail: name-based virtual NIC filter, can miss odd drivers; they would only show up as extra choices.
        var skip = new[] { "virtual", "hyper-v", "vmware", "vpn", "tap-", "wan miniport", "bluetooth", "loopback", "tunnel", "fortinet", "npcap" };
        return NetworkInterface.GetAllNetworkInterfaces()
            // Windows keeps idle Wi-Fi interfaces around, so only the connected one is offered.
            .Where(n => n.NetworkInterfaceType == NetworkInterfaceType.Ethernet || n.NetworkInterfaceType == NetworkInterfaceType.GigabitEthernet
                || (n.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 && n.OperationalStatus == OperationalStatus.Up))
            // The hotspot runs on a Wi-Fi Direct virtual adapter, which the filter would otherwise drop.
            .Where(n => Net.IsThunderbolt(n.Description) || IsHotspot(n) || !skip.Any(s => n.Description.ToLowerInvariant().Contains(s)))
            .Select(n =>
            {
                bool wifi = n.NetworkInterfaceType == NetworkInterfaceType.Wireless80211;
                var v4 = Ipv4(n);
                return new Adapter
                {
                    Name = n.Name, Id = n.Id, Description = n.Description, Up = n.OperationalStatus == OperationalStatus.Up, Speed = n.Speed, Wireless = wifi,
                    Ip = v4 == null ? "" : v4.Address.ToString(),
                    Mask = v4 == null || v4.IPv4Mask == null ? "" : v4.IPv4Mask.ToString()
                };
            })
            // Wi-Fi goes last: it is usually connected, and must not become the default over a cable.
            .OrderBy(a => a.Wireless).ThenByDescending(a => a.Up).ThenByDescending(a => a.Thunderbolt).ThenByDescending(a => a.Hotspot).ThenBy(a => a.Name).ToList();
    }

    static UnicastIPAddressInformation Ipv4(NetworkInterface n)
    {
        return n.GetIPProperties().UnicastAddresses.FirstOrDefault(u => u.Address.AddressFamily == AddressFamily.InterNetwork);
    }

    static bool IsHotspot(NetworkInterface n)
    {
        if (n.NetworkInterfaceType != NetworkInterfaceType.Wireless80211) return false;
        var v4 = Ipv4(n);
        return v4 != null && v4.Address.ToString() == Net.HotspotIp;
    }

    // Addresses Windows has recently talked to, from the ARP table. A row is 24 bytes after the int32 count: address at +16, type at +20.
    public static List<string> NeighborIps()
    {
        const int Dynamic = 3;
        var r = new List<string>();
        int size = 0;
        GetIpNetTable(IntPtr.Zero, ref size, false);
        if (size == 0) return r;
        IntPtr table = Marshal.AllocHGlobal(size);
        try
        {
            // A table that grew since the size check fails here, and the next timer tick tries again.
            if (GetIpNetTable(table, ref size, false) != 0) return r;
            int count = Marshal.ReadInt32(table);
            for (int i = 0; i < count; i++)
            {
                int row = 4 + 24 * i;
                if (Marshal.ReadInt32(table, row + 20) == Dynamic)
                    r.Add(new IPAddress((uint)Marshal.ReadInt32(table, row + 16)).ToString());
            }
        }
        finally { Marshal.FreeHGlobal(table); }
        return r;
    }

    public static Dictionary<string, string> Hotspot(string op, string password = null)
    {
        string output;
        string exe = Path.Combine(Environment.SystemDirectory, @"WindowsPowerShell\v1.0\powershell.exe");
        Exec(exe, "-NoProfile -NonInteractive -Command \"" + Net.HotspotScript(op) + "\"", out output, password);
        return Net.ParseHotspot(output);
    }

    // Registry over GetIPProperties: it still reports static settings while the cable is unplugged.
    public static Config ReadConfig(string id)
    {
        using (var k = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters\Interfaces\" + id))
        {
            if (k == null) return new Config { Dhcp = true };
            Func<string, string> first = n =>
            {
                var v = k.GetValue(n) as string[];
                return v != null && v.Length > 0 ? v[0] : "";
            };
            // ponytail: keeps only the first static IP, multi-IP adapters would need a list here.
            var c = new Config
            {
                Dhcp = Convert.ToInt32(k.GetValue("EnableDHCP", 1)) != 0,
                Ip = first("IPAddress"),
                Mask = first("SubnetMask"),
                Gateway = first("DefaultGateway"),
                Dns = Net.ParseDns(k.GetValue("NameServer", "") as string ?? "")
            };
            if (!Net.IsIp(c.Ip)) c.Dhcp = true;
            return c;
        }
    }

    // Keeps the first backup so applying twice never overwrites the user's original settings.
    public static void BackupOnce(string id)
    {
        var path = BackupPath(id);
        if (File.Exists(path)) return;
        Directory.CreateDirectory(DataDir);
        File.WriteAllLines(path, Net.Serialize(ReadConfig(id)));
    }

    public static Config LoadBackup(string id)
    {
        var path = BackupPath(id);
        return File.Exists(path) ? Net.Deserialize(File.ReadAllLines(path)) : null;
    }

    public static void DeleteBackup(string id) { File.Delete(BackupPath(id)); }

    // Remembers the applied setup so reopening the app (or dropping files on its icon) can send right away.
    public static void SaveSession(string id, string peer, Config c)
    {
        Directory.CreateDirectory(DataDir);
        File.WriteAllLines(SessionPath, new[] { id, peer }.Concat(Net.Serialize(c)));
    }

    public static void DeleteSession() { File.Delete(SessionPath); }

    public static string[] LoadSession()
    {
        try
        {
            var lines = File.ReadAllLines(SessionPath);
            return lines.Length == 7 ? lines : null;
        }
        catch (IOException) { return null; }
    }

    // Returns the first conflicting adapter's name, or null.
    public static string SubnetConflict(string adapterId, string ip, string mask)
    {
        foreach (var n in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (n.Id == adapterId || n.OperationalStatus != OperationalStatus.Up || n.NetworkInterfaceType == NetworkInterfaceType.Loopback) continue;
            foreach (var u in n.GetIPProperties().UnicastAddresses)
                if (u.Address.AddressFamily == AddressFamily.InterNetwork && Net.SameSubnet(u.Address.ToString(), ip, mask))
                    return n.Name;
        }
        return null;
    }

    // Full System32 path: a bare "netsh" would also search the exe's own folder, which matters once elevated.
    static string SystemExe(string name) { return Path.Combine(Environment.SystemDirectory, name + ".exe"); }

    static int Exec(string exe, string args, out string output, string hotspotPassword = null)
    {
        var enc = Encoding.GetEncoding(GetOEMCP());
        var psi = new ProcessStartInfo(exe, args)
        {
            UseShellExecute = false, CreateNoWindow = true,
            RedirectStandardOutput = true, RedirectStandardError = true,
            StandardOutputEncoding = enc, StandardErrorEncoding = enc
        };
        if (hotspotPassword != null) psi.EnvironmentVariables[Net.HotspotPasswordVariable] = hotspotPassword;
        using (var p = Process.Start(psi))
        {
            var o = p.StandardOutput.ReadToEndAsync();
            var e = p.StandardError.ReadToEndAsync();
            p.WaitForExit();
            output = (o.Result + e.Result).Trim();
            return p.ExitCode;
        }
    }

    public static bool FirewallRuleExists(string rule)
    {
        string ignored;
        return Exec(SystemExe("netsh"), "advfirewall firewall show rule name=" + rule, out ignored) == 0;
    }

    public static bool RdpDenied()
    {
        var value = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Terminal Server", "fDenyTSConnections", 1);
        return !(value is int) || (int)value != 0;
    }

    public static bool RdpListening()
    {
        return IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners().Any(e => e.Port == Net.RdpPort);
    }

    // Why this PC cannot accept RDP, or null when it can.
    public static string RdpBlockReason()
    {
        var edition = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "EditionID", null) as string;
        if (Net.IsHomeEdition(edition)) return "這台是 Home 版,無法被遠端連線,但仍可連到別台";
        var policy = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows NT\Terminal Services", "fDenyTSConnections", null);
        if (policy is int && (int)policy == 1) return "公司 IT 政策已停用這台的遠端桌面";
        return null;
    }

    // Remembers the state before the first enable, so turning RDP off never disables what the user had on.
    public static void BackupRdpOnce()
    {
        if (File.Exists(RdpBackupPath)) return;
        var v = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Terminal Server", "fDenyTSConnections", 1);
        Directory.CreateDirectory(DataDir);
        File.WriteAllText(RdpBackupPath, v is int && (int)v == 0 ? "0" : "1");
    }

    public static bool RdpWasDenied()
    {
        return File.Exists(RdpBackupPath) && File.ReadAllText(RdpBackupPath).Trim() == "1";
    }

    public static void DeleteRdpBackup() { File.Delete(RdpBackupPath); }

    // The app runs unelevated so Explorer can drag files onto it (UIPI blocks drops on elevated windows).
    // Only these steps run elevated, by relaunching this exe in --admin mode behind one UAC prompt.
    public static void RunElevated(IEnumerable<string> steps)
    {
        string outFile = Path.GetTempFileName();
        try
        {
            var psi = new ProcessStartInfo(Process.GetCurrentProcess().MainModule.FileName)
            {
                UseShellExecute = true,
                Verb = "runas",
                Arguments = string.Format("--admin \"{0}\" {1}", outFile,
                    Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Join("\n", steps))))
            };
            Process p;
            try { p = Process.Start(psi); }
            catch (Win32Exception ex)
            {
                if (ex.NativeErrorCode == ErrorCancelled) throw new Exception("已取消:變更網路設定需要系統管理員權限。");
                throw;
            }
            using (p)
            {
                p.WaitForExit();
                if (p.ExitCode != 0)
                {
                    string msg = File.ReadAllText(outFile).Trim();
                    throw new Exception(msg == "" ? string.Format(Lang.L("設定失敗(代碼 {0})"), p.ExitCode) : msg);
                }
            }
        }
        finally { File.Delete(outFile); }
    }

    // Entry point of the elevated child. Only netsh, net and reg can run.
    public static int RunSteps(string outFile, string base64)
    {
        foreach (var line in Encoding.UTF8.GetString(Convert.FromBase64String(base64)).Split('\n'))
        {
            var parts = line.Split(new[] { '\t' }, 3);
            if (parts.Length != 3 || (parts[1] != "netsh" && parts[1] != "net" && parts[1] != "reg"))
            {
                File.WriteAllText(outFile, "不允許的指令");
                return 2;
            }
            string output;
            if (Exec(SystemExe(parts[1]), parts[2], out output) != 0 && parts[0] == "!")
            {
                File.WriteAllText(outFile, output == "" ? string.Format(Lang.L("{0} 執行失敗"), parts[1] + " " + parts[2]) : output);
                return 1;
            }
        }
        return 0;
    }
}

class Ui
{
    static readonly string[] ThemeModes = { "auto", "light", "dark" };
    static readonly string[] ThemeKeys = { "Bg", "Card", "Field", "Line", "Text", "Sub", "SegBg", "SegOn", "TrackOff", "Blue", "Red", "Green", "DropOn" };
    static readonly string[] LightPalette = { "#F5F5F7", "#FFFFFF", "#F2F2F5", "#E5E5EA", "#1D1D1F", "#6E6E73", "#E3E3E8", "#FFFFFF", "#D1D1D6", "#007AFF", "#D70015", "#1E8E3E", "#E5F1FF" };
    static readonly string[] DarkPalette = { "#1C1C1E", "#2C2C2E", "#3A3A3C", "#3A3A3C", "#F5F5F7", "#A1A1A6", "#3A3A3C", "#636366", "#48484A", "#0A84FF", "#FF6961", "#30D158", "#12335C" };

    readonly Window w;
    readonly RadioButton roleA, roleB;
    readonly ComboBox adapters;
    readonly ToggleButton firewall, rdpToggle;
    readonly TextBox ip, mask, peer, gateway, dns, sendTo;
    readonly TextBlock status, summary, adapterInfo, dropTitle, peerState, progressText;
    readonly Ellipse peerDot;
    readonly Grid progressRow;
    readonly ProgressBar progress;
    readonly Button dropZone, cancelBtn, themeBtn, langBtn;
    readonly Button[] actions;

    string themeMode = "auto";
    TcpListener listener;
    volatile string allowedPeer;
    string subnetAdapterId;
    bool wifiMode, wifiLost;
    readonly ToggleButton hotspotToggle;
    bool hotspotBusy, hotspotSeen, wantHotspot;
    string hotspotSsid, hotspotPass;
    volatile TcpClient sendClient;
    volatile bool cancelled;
    bool pinging;
    string browsePath = ""; // The peer folder on screen; "" is its drive list.
    int browseRequest;
    System.Windows.Forms.NotifyIcon tray; // Null in a copy opened only to send files.
    System.Windows.Forms.ToolStripItem trayShow, trayQuit;
    volatile bool receiving;
    bool checkingUpdate;
    string downloadedVersion, updateNoteKey, updateNoteColor;
    string[] updateNoteArgs;
    Updater.Release updateAvailable;
    bool installRequested;
    DateTime lastUpdateCheck = DateTime.MinValue;
    bool exiting, restoringForExit;
    WebShare web;
    string webAdapterId;
    bool webOnHotspot;
    Rect? zoomedFrom;

    T F<T>(string name) { return (T)w.FindName(name); }

    public Ui(string[] filesToSend, bool firstCopy)
    {
        using (var s = Assembly.GetExecutingAssembly().GetManifestResourceStream("Main.xaml"))
            w = (Window)XamlReader.Load(s);

        roleA = F<RadioButton>("RoleA"); roleB = F<RadioButton>("RoleB");
        adapters = F<ComboBox>("Adapters");
        firewall = F<ToggleButton>("Firewall");
        ip = F<TextBox>("Ip"); mask = F<TextBox>("Mask"); peer = F<TextBox>("Peer");
        gateway = F<TextBox>("Gateway"); dns = F<TextBox>("Dns"); sendTo = F<TextBox>("SendTo");
        status = F<TextBlock>("Status"); summary = F<TextBlock>("Summary"); adapterInfo = F<TextBlock>("AdapterInfo");
        dropTitle = F<TextBlock>("DropTitle"); peerState = F<TextBlock>("PeerState"); progressText = F<TextBlock>("ProgressText");
        peerDot = F<Ellipse>("PeerDot"); progressRow = F<Grid>("ProgressRow"); progress = F<ProgressBar>("Progress");
        dropZone = F<Button>("DropZone"); cancelBtn = F<Button>("CancelBtn"); themeBtn = F<Button>("ThemeBtn");
        rdpToggle = F<ToggleButton>("RdpToggle");
        actions = new[] { "ApplyBtn", "TestBtn", "RestoreBtn", "DropZone", "RdpBtn", "SpeedBtn", "PeerBrowseBtn", "WebBtn", "HotspotPassBtn" }.Select(F<Button>).ToArray();

        string rdpBlock = Sys.RdpBlockReason();
        if (rdpBlock != null)
        {
            rdpToggle.IsEnabled = false;
            F<TextBlock>("RdpHint").Text = rdpBlock;
        }

        LoadTheme();
        themeBtn.Click += (s, e) =>
        {
            themeMode = ThemeModes[(Array.IndexOf(ThemeModes, themeMode) + 1) % ThemeModes.Length];
            Directory.CreateDirectory(Sys.DataDir);
            File.WriteAllText(Sys.ThemePath, themeMode);
            ApplyTheme();
        };
        SystemEvents.UserPreferenceChanged += (s, e) => w.Dispatcher.BeginInvoke(new Action(ApplyTheme));

        F<TextBlock>("VersionText").Text = "v" + Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
        langBtn = F<Button>("LangBtn");
        langBtn.Click += (s, e) =>
        {
            Lang.En = !Lang.En;
            Lang.Save();
            ApplyLanguage();
            CheckPeer();
        };

        w.MaxHeight = SystemParameters.WorkArea.Height;
        F<Grid>("TitleBar").MouseLeftButtonDown += (s, e) => w.DragMove();
        F<Button>("CloseBtn").Click += (s, e) => w.Close();
        F<Button>("MinBtn").Click += (s, e) => w.WindowState = WindowState.Minimized;
        F<Button>("ZoomBtn").Click += (s, e) => ToggleZoom();
        F<Button>("WinCloseBtn").Click += (s, e) => w.Close();
        F<Button>("WinMinBtn").Click += (s, e) => w.WindowState = WindowState.Minimized;
        F<Button>("WinZoomBtn").Click += (s, e) => ToggleZoom();

        roleA.Checked += (s, e) => SetRole("1", "2");
        roleB.Checked += (s, e) => SetRole("2", "1");
        ip.TextChanged += (s, e) => UpdateLabels();
        peer.TextChanged += (s, e) => UpdateLabels();
        SetRole("1", "2");

        F<Button>("WebBtn").Click += (s, e) => OpenWeb();
        F<Button>("WebDoneBtn").Click += (s, e) => CloseWebOverlay();
        F<Button>("WebQrBtn").Click += (s, e) => ShowWebQr(F<UniformGrid>("WebQr").Visibility != Visibility.Visible);
        F<Button>("WebStopBtn").Click += (s, e) =>
        {
            StopWeb();
            Say("已停止分享。", "Green");
        };
        F<Button>("WebClearBtn").Click += (s, e) =>
        {
            web.Clear();
            ShowWebList();
        };
        F<Button>("WebAddFilesBtn").Click += (s, e) =>
        {
            var d = new OpenFileDialog { Multiselect = true, Title = Lang.L("選擇要給對方下載的檔案") };
            if (d.ShowDialog(w) == true) AddWebItems(d.FileNames);
        };
        F<Button>("WebAddFolderBtn").Click += (s, e) =>
        {
            using (var d = new System.Windows.Forms.FolderBrowserDialog { Description = Lang.L("選擇要給對方下載的資料夾") })
            {
                if (d.ShowDialog() == System.Windows.Forms.DialogResult.OK) AddWebItems(new[] { d.SelectedPath });
            }
        };
        var webDrop = F<Border>("WebDrop");
        webDrop.DragOver += (s, e) =>
        {
            e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
            e.Handled = true;
        };
        webDrop.Drop += (s, e) =>
        {
            var files = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (files != null) AddWebItems(files);
        };
        F<Border>("WebOverlay").PreviewKeyDown += (s, e) =>
        {
            if (e.Key == System.Windows.Input.Key.Escape) CloseWebOverlay();
        };
        F<Button>("AdvancedBtn").Click += (s, e) => OpenAdvanced();
        F<Button>("AdvancedDoneBtn").Click += (s, e) => CloseAdvanced();
        F<Button>("AutoIpBtn").Click += (s, e) => UseAutomaticIp();
        F<Button>("AdapterSettingsBtn").Click += (s, e) =>
        {
            try { Process.Start(Path.Combine(Environment.SystemDirectory, "control.exe"), "ncpa.cpl"); }
            catch (Win32Exception ex) { Say(ex.Message, "Red"); }
        };
        F<Border>("AdvancedOverlay").PreviewKeyDown += (s, e) =>
        {
            if (e.Key == System.Windows.Input.Key.Escape) CloseAdvanced();
        };

        hotspotToggle = F<ToggleButton>("HotspotToggle");
        hotspotToggle.Click += async (s, e) => await RunHotspot(hotspotToggle.IsChecked == true ? "start" : "stop");
        F<Button>("HotspotPassBtn").Click += (s, e) => OpenHotspotPassword();
        F<Button>("HotspotPassCancelBtn").Click += (s, e) => CloseHotspotPassword();
        F<Button>("HotspotPassSaveBtn").Click += (s, e) => SaveHotspotPassword();
        F<Border>("HotspotOverlay").PreviewKeyDown += (s, e) =>
        {
            if (e.Key == System.Windows.Input.Key.Escape) CloseHotspotPassword();
        };
        adapters.SelectionChanged += (s, e) =>
        {
            var a = adapters.SelectedItem as Adapter;
            adapterInfo.Text = a == null ? Lang.L("找不到有線網卡,請確認網卡已啟用") : a.Description;
            if (a == null) return;
            bool wasWifi = wifiMode;
            wifiMode = a.Wireless;
            if (wifiMode) wifiLost = false;
            roleA.IsEnabled = roleB.IsEnabled = !wifiMode;
            ip.IsEnabled = mask.IsEnabled = gateway.IsEnabled = dns.IsEnabled = !wifiMode;
            // Refreshed on every network change, since the router can hand out a new address.
            if (wifiMode) { ip.Text = a.Ip; mask.Text = a.Mask; }
            if (a.Id != subnetAdapterId)
            {
                subnetAdapterId = a.Id;
                Say("網卡已切換,請兩台都按一次「一鍵設定」。", "Sub");
                if (wifiMode)
                {
                    peer.Text = gateway.Text = dns.Text = "";
                }
                // Addresses typed in the advanced settings stay as they are.
                else if (wasWifi || (Net.IsDefaultAddress(ip.Text) && Net.IsDefaultAddress(peer.Text)))
                {
                    mask.Text = "255.255.255.0";
                    if (roleA.IsChecked == true) SetRole("1", "2");
                    else SetRole("2", "1");
                }
            }
            // A PC on the hotspot always knows the host's address. Joining keeps the same adapter, so this runs on every refresh.
            if (a.HotspotClient && peer.Text.Trim() != Net.HotspotIp) peer.Text = Net.HotspotIp;
            UpdateLabels();
        };
        RefreshAdapters();
        NetworkChange.NetworkAddressChanged += (s, e) => w.Dispatcher.BeginInvoke(new Action(() =>
        {
            RefreshAdapters();
            // The Thunderbolt adapter can appear only once the cable is in, which may be after this app started.
            if (allowedPeer == null || wifiLost) RestoreSession();
        }));

        F<Button>("ApplyBtn").Click += (s, e) => Apply();
        F<Button>("TestBtn").Click += (s, e) => Test();
        F<Button>("RestoreBtn").Click += (s, e) => Restore();
        F<Button>("InboxBtn").Click += (s, e) => OpenInbox();
        F<Button>("RdpBtn").Click += (s, e) => ConnectRdp();
        F<Button>("SpeedBtn").Click += (s, e) => RunSpeedTest();
        F<Button>("InboxPickBtn").Click += (s, e) => PickInbox();
        F<Button>("InboxResetBtn").Click += (s, e) => ResetInbox();
        try { sendTo.Text = File.ReadAllText(Sys.SendToPath).Trim(); }
        catch (IOException) { }
        sendTo.TextChanged += (s, e) =>
        {
            try
            {
                Directory.CreateDirectory(Sys.DataDir);
                File.WriteAllText(Sys.SendToPath, sendTo.Text.Trim());
            }
            catch (IOException) { } // Only the next start loses the setting; the box still applies now.
        };
        F<Button>("PeerResetBtn").Click += (s, e) => sendTo.Text = "";
        F<Button>("PeerBrowseBtn").Click += (s, e) => OpenBrowse();
        F<Button>("BrowseUpBtn").Click += (s, e) => BrowseTo(Path.GetDirectoryName(browsePath) ?? "");
        F<Button>("BrowseCancelBtn").Click += (s, e) => CloseBrowse();
        F<Button>("BrowseSelectBtn").Click += (s, e) =>
        {
            sendTo.Text = browsePath;
            CloseBrowse();
        };
        F<Border>("BrowseOverlay").PreviewKeyDown += (s, e) =>
        {
            if (e.Key == System.Windows.Input.Key.Escape) CloseBrowse();
        };
        // Enter here must not reach the default button, which is One-Click Setup.
        sendTo.PreviewKeyDown += (s, e) =>
        {
            if (e.Key == System.Windows.Input.Key.Enter) e.Handled = true;
        };
        ShowInbox();
        cancelBtn.Click += (s, e) => CancelSend();

        F<Button>("SettingsBtn").Click += (s, e) => OpenSettings();
        F<Button>("SettingsDoneBtn").Click += (s, e) => CloseSettings();
        F<Border>("SettingsOverlay").PreviewKeyDown += (s, e) =>
        {
            if (e.Key == System.Windows.Input.Key.Escape) CloseSettings();
        };
        RoutedEventHandler rdpChanged = (s, e) => ShowRdpStatus();
        rdpToggle.Checked += rdpChanged;
        rdpToggle.Unchecked += rdpChanged;
        BindOption("AutoUpdateToggle", () => Sys.AutoUpdate, on => Sys.AutoUpdate = on);
        var checkUpdate = F<Button>("CheckUpdateBtn");
        checkUpdate.IsEnabled = firstCopy; // A send-only window must not replace the program under the main one.
        checkUpdate.Click += async (s, e) => await UpdateWork(true, () => updateAvailable != null ? InstallUpdate(updateAvailable, true) : CheckForUpdate(true));
        UpdateNote(false, "Sub", "目前版本 v{0}", CurrentVersion().ToString(3));
        if (firstCopy) StartUpdateTimer();
        BindOption("AutoStartToggle", () => Sys.AutoStart, on => Sys.AutoStart = on);
        BindOption("QuitOnCloseToggle", () => Sys.QuitOnClose, on => Sys.QuitOnClose = on);
        if (Sys.WindowsUi)
        {
            F<RadioButton>("UiWindows").IsChecked = true;
            ApplyUiStyle(true);
        }
        F<RadioButton>("UiWindows").Checked += (s, e) => SetUiStyle(true);
        F<RadioButton>("UiApple").Checked += (s, e) => SetUiStyle(false);
        // Only one of the two can be on; both off means quitting leaves the settings as they are.
        var askRestore = F<ToggleButton>("AskRestoreToggle");
        var autoRestore = F<ToggleButton>("AutoRestoreToggle");
        BindOption("AskRestoreToggle", () => Sys.AskRestore, on =>
        {
            Sys.AskRestore = on;
            if (on) autoRestore.IsChecked = false;
        });
        BindOption("AutoRestoreToggle", () => Sys.AutoRestore, on =>
        {
            Sys.AutoRestore = on;
            if (on) askRestore.IsChecked = false;
        });
        F<Button>("ExitRestoreBtn").Click += (s, e) =>
        {
            CloseExitPrompt();
            RestoreAndQuit();
        };
        F<Button>("ExitOnlyBtn").Click += (s, e) => Quit();
        F<Button>("ExitCancelBtn").Click += (s, e) => CloseExitPrompt();
        F<Border>("ExitOverlay").PreviewKeyDown += (s, e) =>
        {
            if (e.Key == System.Windows.Input.Key.Escape) CloseExitPrompt();
        };
        // Rewrites the saved path, in case the program was moved since it was turned on.
        try { if (Sys.AutoStart) Sys.AutoStart = true; }
        catch (Exception) { }
        if (firstCopy) CreateTray();
        w.Closing += (s, e) =>
        {
            if (tray == null || exiting) return;
            e.Cancel = true;
            if (Sys.QuitOnClose)
            {
                // Queued: WPF throws if a closing window is shown or closed again from inside this event.
                w.Dispatcher.BeginInvoke(new Action(RequestExit));
                return;
            }
            w.Hide();
            tray.ShowBalloonTip(5000, Lang.L("LAN 互連"), Lang.L("LAN 互連仍在系統匣執行,可以繼續收檔。右鍵按圖示可以展開視窗或退出。"), System.Windows.Forms.ToolTipIcon.Info);
        };
        w.Closed += (s, e) => Application.Current.Shutdown();

        dropZone.Click += (s, e) => PickFiles();
        dropZone.DragEnter += (s, e) => dropZone.Tag = e.Data.GetDataPresent(DataFormats.FileDrop) ? "hi" : null;
        dropZone.DragLeave += (s, e) => dropZone.Tag = null;
        dropZone.DragOver += (s, e) =>
        {
            e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
            e.Handled = true;
        };
        dropZone.Drop += (s, e) =>
        {
            dropZone.Tag = null;
            var files = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (files != null) SendFiles(files);
        };

        Say("兩台各開一次這個工具,一台選 A、一台選 B,接上網路線或 Thunderbolt 線後按「一鍵設定」。", "Sub");
        RestoreSession();
        ApplyLanguage();

        // Async work waits for Loaded: before Application.Run there is no dispatcher context to resume on.
        w.Loaded += (s, e) =>
        {
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
            timer.Tick += (s2, e2) =>
            {
                DetectHotspotPeer();
                CheckPeer();
            };
            timer.Start();
            CheckPeer();
            if (filesToSend.Length > 0) SendFiles(filesToSend);
        };
    }

    public void Run(bool hidden)
    {
        var app = new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
        app.SessionEnding += (s, e) => exiting = true;
        app.Exit += (s, e) => { if (tray != null) tray.Dispose(); };
        if (tray != null)
        {
            // Another launch of the program signals this to bring up the window.
            var signal = new EventWaitHandle(false, EventResetMode.AutoReset, Program.ShowSignalName);
            var thread = new Thread(() =>
            {
                while (true)
                {
                    signal.WaitOne();
                    w.Dispatcher.BeginInvoke(new Action(ShowWindow));
                }
            }) { IsBackground = true };
            thread.Start();
        }
        if (hidden) app.Run();
        else app.Run(w);
    }

    void CreateTray()
    {
        var menu = new System.Windows.Forms.ContextMenuStrip();
        trayShow = menu.Items.Add("", null, (s, e) => ShowWindow());
        // Queued so its async restore resumes on the WPF dispatcher, not the menu's WinForms context.
        trayQuit = menu.Items.Add("", null, (s, e) => w.Dispatcher.BeginInvoke(new Action(RequestExit)));
        tray = new System.Windows.Forms.NotifyIcon
        {
            Icon = System.Drawing.Icon.ExtractAssociatedIcon(Sys.ProgramPath),
            ContextMenuStrip = menu,
            Visible = true
        };
        tray.MouseClick += (s, e) =>
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left) ShowWindow();
        };
    }

    void ShowWindow()
    {
        w.Show();
        if (w.WindowState == WindowState.Minimized) w.WindowState = WindowState.Normal;
        w.Activate();
    }

    void ShowRdpStatus()
    {
        if (!rdpToggle.IsEnabled) return; // Already explains why this PC can't accept connections.
        F<TextBlock>("RdpHint").Text = Lang.L(rdpToggle.IsChecked == true
            ? Net.RdpStatus(Sys.RdpDenied(), Sys.RdpListening())
            : "對方可用這台的帳號密碼登入並操作這台");
    }

    // Created here and not on Loaded, which never fires while starting hidden in the tray.
    void StartUpdateTimer()
    {
        var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(30) };
        timer.Tick += async (s, e) =>
        {
            timer.Interval = TimeSpan.FromHours(1);
            if (!Sys.AutoUpdate) return;
            if (downloadedVersion != null || DateTime.Now - lastUpdateCheck > TimeSpan.FromHours(24)) await UpdateWork(false, () => CheckForUpdate(false));
        };
        timer.Start();
    }

    // A restart would cut off anything in progress.
    bool IsIdle()
    {
        return !receiving && sendClient == null && web == null && hotspotToggle.IsChecked != true && actions[0].IsEnabled;
    }

    static Version CurrentVersion() { return Assembly.GetExecutingAssembly().GetName().Version; }

    // Shown under the switch in Settings. The status line also gets it when Settings is closed and this is worth interrupting for.
    void UpdateNote(bool announce, string color, string key, params string[] args)
    {
        updateNoteKey = key;
        updateNoteColor = color;
        updateNoteArgs = args;
        ShowUpdateNote();
        if (announce && F<Border>("SettingsOverlay").Visibility != Visibility.Visible)
            Say(F<TextBlock>("UpdateStatus").Text, color);
    }

    void ShowUpdateNote()
    {
        var note = F<TextBlock>("UpdateStatus");
        note.Text = string.Format(Lang.L(updateNoteKey), updateNoteArgs.Select(a => (object)Lang.L(a)).ToArray());
        note.SetResourceReference(TextBlock.ForegroundProperty, updateNoteColor);
    }

    // The same button checks, and installs once a new version is known.
    void ShowUpdateButton()
    {
        var button = F<Button>("CheckUpdateBtn");
        button.Content = Lang.L(updateAvailable != null ? "立即更新" : "檢查更新");
        button.SetResourceReference(Control.BackgroundProperty, updateAvailable != null ? "Blue" : "Field");
        if (updateAvailable != null) button.Foreground = Brushes.White;
        else button.ClearValue(Control.ForegroundProperty);
    }

    async Task CheckForUpdate(bool asked)
    {
        if (asked) UpdateNote(false, "Sub", "正在檢查更新…");
        var current = CurrentVersion();
        var latest = await Task.Run(() => Updater.Latest());
        lastUpdateCheck = DateTime.Now;
        if (latest == null)
        {
            if (asked) UpdateNote(true, "Red", "GitHub 上沒有可用的更新。");
            return;
        }
        updateAvailable = Updater.IsNewer(current, latest.Version) ? latest : null;
        ShowUpdateButton();
        if (updateAvailable == null) UpdateNote(asked, "Green", "已經是最新版本 v{0}({1} 檢查)", current.ToString(3), lastUpdateCheck.ToString("MM/dd HH:mm"));
        // Checking by hand only reports: installing is a separate choice.
        else if (asked) UpdateNote(true, "Green", "發現新版 v{0},按「立即更新」安裝。", latest.Version);
        else await InstallUpdate(latest, false);
    }

    async Task InstallUpdate(Updater.Release release, bool asked)
    {
        if (asked) installRequested = true;
        if (downloadedVersion != release.Version)
        {
            UpdateNote(false, "Sub", "正在下載新版 v{0}…", release.Version);
            await Task.Run(() => Updater.Download(release, Sys.ProgramPath));
            downloadedVersion = release.Version;
        }
        // A window in use isn't restarted unless the user asked for this.
        if (IsIdle() && (asked || !w.IsVisible))
        {
            UpdateNote(true, "Sub", "正在安裝新版 v{0},程式會重新啟動…", release.Version);
            bool hidden = !w.IsVisible;
            downloadedVersion = null; // After a failed install there is no copy left to put in place on quit.
            await Task.Run(() => Updater.InstallAndRestart(Sys.ProgramPath, hidden));
            Quit();
        }
        else UpdateNote(true, "Green", "新版 v{0} 已下載,關閉程式時會自動安裝。", release.Version);
    }

    // Automatic checks stay quiet on failure: a PC on a direct cable often has no internet.
    async Task UpdateWork(bool asked, Func<Task> work)
    {
        if (checkingUpdate) return;
        checkingUpdate = true;
        var button = F<Button>("CheckUpdateBtn");
        button.IsEnabled = false;
        try { await work(); }
        catch (WebException)
        {
            if (asked) UpdateNote(true, "Red", "無法連線到 GitHub,請確認這台能上網後再試一次。");
        }
        catch (Exception ex)
        {
            if (asked || ex is InvalidOperationException) UpdateNote(true, "Red", "更新失敗:{0}", ex.Message);
        }
        finally
        {
            checkingUpdate = false;
            button.IsEnabled = true;
        }
    }

    void Quit()
    {
        // Best effort: the next launch simply keeps the current version and checks again.
        // Turning automatic updates off also cancels an install nobody asked for.
        if (downloadedVersion != null && (Sys.AutoUpdate || installRequested) && Updater.HasPending(Sys.ProgramPath))
        {
            try { Updater.Swap(Sys.ProgramPath); }
            catch (Exception) { }
        }
        exiting = true;
        Application.Current.Shutdown();
    }

    void RequestExit()
    {
        if (restoringForExit) return;
        // Nothing was set up, so there is nothing to offer.
        bool anyBackup = ((List<Adapter>)adapters.ItemsSource).Any(x => Sys.LoadBackup(x.Id) != null);
        if (Sys.LoadSession() == null && hotspotToggle.IsChecked != true && web == null && !anyBackup) Quit();
        else if (Sys.AutoRestore) RestoreAndQuit();
        else if (Sys.AskRestore) ShowExitPrompt(null);
        else Quit();
    }

    void RestoreAndQuit()
    {
        restoringForExit = true;
        Guard(async () =>
        {
            try
            {
                await RestoreWork();
                Quit();
            }
            // The prompt keeps "Quit Only" in reach, so a refused restore can't trap the user in the app.
            catch (Exception ex) { ShowExitPrompt(ex.Message); }
            restoringForExit = false;
        });
    }

    void ShowExitPrompt(string problem)
    {
        CloseHotspotPassword();
        CloseWebOverlay();
        CloseAdvanced();
        CloseBrowse();
        CloseSettings();
        ShowWindow();
        F<TextBlock>("ExitMessage").Text = problem != null ? Lang.L(problem)
            : Lang.L("會恢復原本的 IP,移除防火牆規則並關閉熱點。之後要傳檔,得再按一次「一鍵設定」。");
        // Disabled underneath so Enter can't reach One-Click Setup, the default button.
        F<ScrollViewer>("Scroll").IsEnabled = false;
        F<Border>("ExitOverlay").Visibility = Visibility.Visible;
        F<Button>("ExitCancelBtn").Focus();
    }

    void CloseExitPrompt()
    {
        F<Border>("ExitOverlay").Visibility = Visibility.Collapsed;
        F<ScrollViewer>("Scroll").IsEnabled = true;
    }

    void DetectHotspotPeer()
    {
        var a = adapters.SelectedItem as Adapter;
        if (a == null || !a.Hotspot) return;
        string current = peer.Text.Trim();
        string found = Net.HotspotPeer(Sys.NeighborIps(), current);
        if (found == null || found == current) return;
        peer.Text = found;
        if (found != allowedPeer) Say(string.Format(Lang.L("偵測到另一台 {0},請兩台都按一次「一鍵設定」。"), found), "Green");
    }

    // Sized by hand: WindowState.Maximized on a borderless transparent window covers the taskbar.
    void ToggleZoom()
    {
        Rect frame;
        if (zoomedFrom != null)
        {
            frame = zoomedFrom.Value;
            zoomedFrom = null;
        }
        else
        {
            zoomedFrom = new Rect(w.Left, w.Top, w.ActualWidth, w.ActualHeight);
            var area = System.Windows.Forms.Screen.FromHandle(new System.Windows.Interop.WindowInteropHelper(w).Handle).WorkingArea;
            var fromDevice = PresentationSource.FromVisual(w).CompositionTarget.TransformFromDevice;
            frame = new Rect(fromDevice.Transform(new Point(area.Left, area.Top)), fromDevice.Transform(new Point(area.Right, area.Bottom)));
        }
        w.SizeToContent = SizeToContent.Manual;
        // MaxHeight holds the primary screen's height, which a taller screen would exceed.
        w.MaxHeight = double.PositiveInfinity;
        w.Left = frame.Left;
        w.Top = frame.Top;
        w.Width = frame.Width;
        w.Height = frame.Height;
        F<Button>("WinZoomBtn").Content = zoomedFrom != null ? "" : "";
    }

    void SetUiStyle(bool windows)
    {
        ApplyUiStyle(windows);
        try { Sys.WindowsUi = windows; }
        catch (Exception) { } // Only the next start loses the setting; the window already changed.
    }

    void ApplyUiStyle(bool windows)
    {
        F<StackPanel>("AppleCaption").Visibility = windows ? Visibility.Collapsed : Visibility.Visible;
        F<StackPanel>("WindowsCaption").Visibility = windows ? Visibility.Visible : Visibility.Collapsed;
        F<StackPanel>("TitleText").HorizontalAlignment = windows ? HorizontalAlignment.Left : HorizontalAlignment.Center;
        // Leaves room for the three 46px buttons, which sit outside the title bar's 18px margin.
        F<StackPanel>("TitleTools").Margin = new Thickness(0, 0, windows ? 124 : 0, 0);
    }

    void OpenWeb()
    {
        if (web != null) { ShowWebOverlay(); return; }
        var a = adapters.SelectedItem as Adapter;
        string problem = a == null ? "找不到有線網卡"
            : a.Wireless && !a.Hotspot ? "網頁下載只支援網路線、Thunderbolt 或 Wi-Fi 熱點,請先選對網卡。"
            : !a.Up ? "請先接上網路線或 Thunderbolt 線。" : null;
        if (problem != null) { Say(problem, "Red"); return; }
        Guard(async () =>
        {
            Say("設定中…請在跳出的視窗按「是」。", "Sub");
            await Task.Run(() =>
            {
                if (a.Hotspot)
                {
                    if (!Sys.FirewallRuleExists(Net.WebHotspotRuleName)) Sys.RunElevated(Net.WebHotspotSteps());
                    return;
                }
                bool toAuto = !Sys.ReadConfig(a.Id).Dhcp;
                if (!toAuto && Sys.FirewallRuleExists(Net.WebRuleName)) return;
                if (toAuto) Sys.BackupOnce(a.Id);
                Sys.RunElevated(Net.WebSteps(a.Name, toAuto));
            });
            // Started only after the rule exists, so Windows doesn't pop its own firewall prompt.
            web = new WebShare(IPAddress.Any, Net.WebPort, Net.IsWebClient, name => w.Dispatcher.BeginInvoke(new Action(() =>
                Say(string.Format(Lang.L("對方開始下載:{0}"), name), "Green"))), () => Sys.Inbox, path => w.Dispatcher.BeginInvoke(new Action(() =>
                Say(string.Format(Lang.L("收到 {0} 個檔案,存在 {1}。"), 1, Path.GetDirectoryName(path)), "Green"))));
            webAdapterId = a.Id;
            webOnHotspot = a.Hotspot;
            ShowWebOverlay();
        });
    }

    void AddWebItems(string[] paths)
    {
        var share = web;
        Guard(async () =>
        {
            // Listing a large folder takes a while.
            await Task.Run(() => share.Add(paths));
            ShowWebList();
        });
    }

    void ShowWebList() { F<TextBlock>("WebList").Text = web.Summary(); }

    void UpdateWebUrl()
    {
        if (web == null) return;
        // Windows can recreate the hotspot's adapter when it restarts, so that one is found by what it is.
        var a = ((List<Adapter>)adapters.ItemsSource).FirstOrDefault(x => webOnHotspot ? x.Hotspot : x.Id == webAdapterId);
        F<TextBlock>("WebUrl").Text = a != null && (a.Hotspot || Net.IsLinkLocal(a.Ip))
            ? "http://" + a.Ip + ":" + Net.WebPort
            : Lang.L("正在取得 IP…連線後最多約一分鐘");
        F<TextBlock>("WebHint").Text = !webOnHotspot ? Lang.L("在另一台的瀏覽器輸入這個網址,對方不用裝工具,也不用改設定。")
            : hotspotSsid != null ? string.Format(Lang.L("另一台先連上熱點 {0}(密碼 {1}),再用瀏覽器輸入這個網址。"), hotspotSsid, hotspotPass)
            : Lang.L("另一台先連上這台的熱點,再用瀏覽器輸入這個網址。");
        F<TextBlock>("WebHint").Text += Lang.L("對方也能從網頁上傳檔案,會存到收件位置。");
        if (!webOnHotspot) return;
        F<Image>("WifiQrImage").Source = hotspotSsid == null ? null : QrImage(Net.WifiQrText(hotspotSsid, hotspotPass));
        F<Image>("UrlQrImage").Source = a == null ? null : QrImage("http://" + a.Ip + ":" + Net.WebPort);
    }

    // One pixel per module plus the white margin scanners need; the Image scales it up without blurring.
    static System.Windows.Media.Imaging.BitmapSource QrImage(string text)
    {
        const int Margin = 4;
        var code = Qr.Encode(text);
        if (code == null) return null;
        int n = code.GetLength(0), size = n + 2 * Margin;
        var pixels = Enumerable.Repeat((byte)255, size * size).ToArray();
        for (int y = 0; y < n; y++)
            for (int x = 0; x < n; x++)
                if (code[y, x]) pixels[(y + Margin) * size + x + Margin] = 0;
        return System.Windows.Media.Imaging.BitmapSource.Create(size, size, 96, 96, PixelFormats.Gray8, null, pixels, size);
    }

    void ShowWebQr(bool show)
    {
        F<UniformGrid>("WebQr").Visibility = show ? Visibility.Visible : Visibility.Collapsed;
        F<StackPanel>("WebFiles").Visibility = show ? Visibility.Collapsed : Visibility.Visible;
        F<Button>("WebQrBtn").Content = Lang.L(show ? "檔案清單" : "手機 QR Code");
    }

    void ShowWebOverlay()
    {
        UpdateWebUrl();
        ShowWebList();
        ShowWebQr(false);
        F<Button>("WebQrBtn").Visibility = webOnHotspot ? Visibility.Visible : Visibility.Collapsed;
        // Disabled underneath so Enter can't reach One-Click Setup, the default button.
        F<ScrollViewer>("Scroll").IsEnabled = false;
        F<Border>("WebOverlay").Visibility = Visibility.Visible;
        F<Button>("WebDoneBtn").Focus();
    }

    void CloseWebOverlay()
    {
        F<Border>("WebOverlay").Visibility = Visibility.Collapsed;
        F<ScrollViewer>("Scroll").IsEnabled = true;
    }

    // The firewall rule and automatic address stay until Restore or One-Click Setup, which need the admin prompt.
    void StopWeb()
    {
        CloseWebOverlay();
        if (web == null) return;
        web.Stop();
        web = null;
    }

    void OpenAdvanced()
    {
        // Disabled underneath so Enter can't reach One-Click Setup, the default button.
        F<ScrollViewer>("Scroll").IsEnabled = false;
        F<Border>("AdvancedOverlay").Visibility = Visibility.Visible;
        if (wifiMode) peer.Focus();
        else ip.Focus();
    }

    void CloseAdvanced()
    {
        F<Border>("AdvancedOverlay").Visibility = Visibility.Collapsed;
        F<ScrollViewer>("Scroll").IsEnabled = true;
    }

    void OpenSettings()
    {
        CloseHotspotPassword();
        CloseWebOverlay();
        CloseAdvanced();
        CloseBrowse();
        // Disabled underneath so Enter can't reach One-Click Setup, the default button.
        F<ScrollViewer>("Scroll").IsEnabled = false;
        F<Border>("SettingsOverlay").Visibility = Visibility.Visible;
        F<Button>("SettingsDoneBtn").Focus();
    }

    void CloseSettings()
    {
        F<Border>("SettingsOverlay").Visibility = Visibility.Collapsed;
        F<ScrollViewer>("Scroll").IsEnabled = true;
    }

    void BindOption(string name, Func<bool> load, Action<bool> save)
    {
        var toggle = F<ToggleButton>(name);
        toggle.IsChecked = load();
        RoutedEventHandler changed = (s, e) =>
        {
            try { save(toggle.IsChecked == true); }
            catch (Exception ex)
            {
                Say(Lang.L("無法儲存設定:") + ex.Message, "Red");
                toggle.IsChecked = load();
            }
        };
        toggle.Checked += changed;
        toggle.Unchecked += changed;
    }

    void LoadTheme()
    {
        try
        {
            var saved = File.ReadAllText(Sys.ThemePath).Trim();
            if (ThemeModes.Contains(saved)) themeMode = saved;
        }
        catch (IOException) { }
        ApplyTheme();
    }

    static bool SystemIsDark()
    {
        var v = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme", 1);
        return v is int && (int)v == 0;
    }

    void ApplyTheme()
    {
        bool dark = themeMode == "dark" || (themeMode == "auto" && SystemIsDark());
        var palette = dark ? DarkPalette : LightPalette;
        for (int i = 0; i < ThemeKeys.Length; i++)
            w.Resources[ThemeKeys[i]] = new BrushConverter().ConvertFromString(palette[i]);
        string label = Lang.L(themeMode == "auto" ? "外觀:跟隨系統" : themeMode == "light" ? "外觀:淺色" : "外觀:深色");
        themeBtn.ToolTip = label + Lang.L("(按一下切換)");
        AutomationPropertiesName(themeBtn, label);
    }

    void ApplyLanguage()
    {
        Translate(w);
        w.Title = Lang.Swap(w.Title);
        UpdateLabels();
        ShowHotspotInfo(hotspotSsid, hotspotPass);
        ShowUpdateNote();
        RefreshAdapters();
        ApplyTheme();
        // The button names the language it switches to.
        if (tray != null)
        {
            tray.Text = Lang.L("LAN 互連");
            trayShow.Text = Lang.L("展開視窗");
            trayQuit.Text = Lang.L("退出");
        }
        langBtn.Content = Lang.En ? "中文" : "EN";
        langBtn.ToolTip = Lang.En ? "切換為中文" : "Switch to English";
        AutomationPropertiesName(langBtn, (string)langBtn.ToolTip);
    }

    // ponytail: a status message built with string.Format keeps its old language until the next message.
    static void Translate(object o)
    {
        var element = o as FrameworkElement;
        if (element != null)
        {
            var tip = element.ToolTip as string;
            if (tip != null) element.ToolTip = Lang.Swap(tip);
            var name = System.Windows.Automation.AutomationProperties.GetName(element);
            if (!string.IsNullOrEmpty(name)) AutomationPropertiesName(element, Lang.Swap(name));
        }
        var text = o as TextBlock;
        if (text != null) text.Text = Lang.Swap(text.Text);
        var content = o as ContentControl;
        if (content != null && content.Content is string) content.Content = Lang.Swap((string)content.Content);

        var parent = o as DependencyObject;
        if (parent == null) return;
        foreach (var child in LogicalTreeHelper.GetChildren(parent).Cast<object>().ToList()) Translate(child);
    }

    static void AutomationPropertiesName(DependencyObject o, string name)
    {
        System.Windows.Automation.AutomationProperties.SetName(o, name);
    }

    void SetRole(string me, string other)
    {
        string prefix = Net.SubnetPrefix(adapters.SelectedItem as Adapter);
        ip.Text = prefix + me;
        peer.Text = prefix + other;
    }

    void UpdateLabels()
    {
        var selected = adapters.SelectedItem as Adapter;
        summary.Text = wifiMode && selected != null && selected.Hotspot
            ? string.Format(Lang.L("這台 {0}。另一台連上這個熱點後,會自動填入對方 IP。"), ip.Text)
            : wifiMode
            ? string.Format(Lang.L("這台 {0}。兩台連同一個 Wi-Fi,在進階設定的「對方 IP」填另一台顯示的 IP。Wi-Fi 的 IP 不會被更改。"), ip.Text)
            : string.Format(Lang.L("這台 {0}  ⇄  對方 {1}。不設閘道,不影響 Wi-Fi 上網。"), ip.Text, peer.Text);
        dropTitle.Text = Lang.L(wifiMode ? "對方電腦" : roleA.IsChecked == true ? "電腦 B" : "電腦 A");
    }

    void RefreshAdapters()
    {
        var current = adapters.SelectedItem as Adapter;
        var list = Sys.Adapters();
        // A Wi-Fi drop removes it from the list, so remember to go back to it when it reconnects.
        if (current != null && current.Wireless && !list.Any(x => x.Id == current.Id)) wifiLost = true;
        adapters.ItemsSource = list;
        var hotspot = list.FirstOrDefault(a => a.Hotspot);
        // Only the moment after turning it on: later the user may choose another adapter.
        adapters.SelectedItem = (wantHotspot ? hotspot : null) ?? Net.PickAdapter(list, current == null ? null : current.Id);
        if (hotspot != null) wantHotspot = false;
        if (hotspot != null) hotspotSeen = true;
        UpdateWebUrl();
        if (hotspotBusy) return;
        if (hotspot != null)
        {
            hotspotToggle.IsChecked = true;
            if (hotspotSsid == null)
            {
                // Queued: this also runs during startup, when async work has no dispatcher context to resume on.
                hotspotBusy = true;
                w.Dispatcher.BeginInvoke(new Action(async () => await RunHotspot("status")));
            }
        }
        else if (hotspotSeen)
        {
            // Windows also turns it off by itself after a few idle minutes.
            hotspotSeen = false;
            hotspotToggle.IsChecked = false;
            ShowHotspotInfo(null, null);
        }
        if (list.Count == 0) adapterInfo.Text = Lang.L("找不到有線網卡,請確認網卡已啟用");
    }

    void ShowHotspotInfo(string ssid, string pass)
    {
        hotspotSsid = ssid;
        hotspotPass = pass;
        F<TextBlock>("HotspotInfo").Text = ssid == null
            ? Lang.L("沒有共同 Wi-Fi 時,讓另一台直接連到這台")
            : string.Format(Lang.L("熱點名稱:{0}  密碼:{1}"), ssid, pass);
        UpdateWebUrl();
    }

    // True when Windows accepted it.
    async Task<bool> RunHotspot(string op, string password = null)
    {
        bool user = op != "status", changingPassword = op == "password", ok = false;
        hotspotBusy = true;
        hotspotToggle.IsEnabled = false;
        if (user) Say(changingPassword ? "正在更新熱點密碼…" : op == "start" ? "正在開啟熱點…" : "正在關閉熱點…", "Sub");
        try
        {
            var r = await Task.Run(() => Sys.Hotspot(op, password));
            string error = Net.HotspotError(r);
            if (error != null) throw new Exception(error);
            bool on = op != "stop" && r["STATE"] != "Off";
            if (user && !changingPassword) wantHotspot = op == "start";
            hotspotToggle.IsChecked = on;
            ShowHotspotInfo(on ? r["SSID"] : null, on ? r["PASS"] : null);
            if (user) Say(changingPassword ? "熱點密碼已更新。" : on ? "熱點已開啟。請從另一台的 Wi-Fi 清單連上它,這台會自動填入對方 IP。" : "熱點已關閉。", "Green");
            ok = true;
        }
        catch (Exception ex)
        {
            if (user)
            {
                Say(string.Format(Lang.L("無法切換熱點:{0}。請確認 Wi-Fi 已開啟且這台已連上網路,或公司政策未停用熱點。"), ex.Message), "Red");
                if (!changingPassword) hotspotToggle.IsChecked = op == "stop";
            }
        }
        hotspotToggle.IsEnabled = true;
        hotspotBusy = false;
        if (op == "start") RefreshAdapters();
        return ok;
    }

    void OpenHotspotPassword()
    {
        F<TextBox>("HotspotPass").Text = hotspotPass ?? "";
        F<TextBlock>("HotspotPassHint").Text = Lang.L("8 到 63 個英文、數字或符號。會一併改掉 Windows 行動熱點的密碼;熱點開著時會重新啟動,已連上的裝置要用新密碼重連。");
        F<TextBlock>("HotspotPassHint").SetResourceReference(TextBlock.ForegroundProperty, "Sub");
        // Disabled underneath so Enter can't reach One-Click Setup, the default button.
        F<ScrollViewer>("Scroll").IsEnabled = false;
        F<Border>("HotspotOverlay").Visibility = Visibility.Visible;
        F<TextBox>("HotspotPass").Focus();
    }

    void CloseHotspotPassword()
    {
        F<Border>("HotspotOverlay").Visibility = Visibility.Collapsed;
        F<ScrollViewer>("Scroll").IsEnabled = true;
    }

    void SaveHotspotPassword()
    {
        string password = F<TextBox>("HotspotPass").Text;
        if (!Net.IsHotspotPassword(password))
        {
            // This overlay covers the status line, so the problem is shown in it.
            var hint = F<TextBlock>("HotspotPassHint");
            hint.Text = Lang.L("密碼要 8 到 63 個英文、數字或符號。");
            hint.SetResourceReference(TextBlock.ForegroundProperty, "Red");
            return;
        }
        CloseHotspotPassword();
        bool wasOn = hotspotToggle.IsChecked == true;
        Guard(async () =>
        {
            if (!await RunHotspot("password", password)) return;
            // A running hotspot keeps the old password until it restarts.
            if (wasOn && await RunHotspot("stop")) await RunHotspot("start");
        });
    }

    void Say(string text, string colorKey)
    {
        status.Text = Lang.L(text);
        status.SetResourceReference(TextBlock.ForegroundProperty, colorKey);
    }

    // Disables the buttons during work and turns any exception into a readable message.
    async void Guard(Func<Task> work)
    {
        foreach (var b in actions) b.IsEnabled = false;
        try { await work(); }
        catch (Exception ex) { Say(ex.Message, "Red"); }
        foreach (var b in actions) b.IsEnabled = true;
    }

    void RestoreSession()
    {
        var s = Sys.LoadSession();
        if (s == null) return;
        var c = Net.Deserialize(s.Skip(2).ToArray());
        var list = (List<Adapter>)adapters.ItemsSource;
        var a = list.FirstOrDefault(x => x.Id == s[0]);
        if (a == null) return;
        // On another Wi-Fi network the saved address could be a stranger, so don't start receiving from it.
        bool sameNetwork = a.Wireless
            ? Net.IsIp(a.Ip) && Net.IsMask(a.Mask) && Net.SameSubnet(a.Ip, s[1], a.Mask)
            : Net.SameAddress(Sys.ReadConfig(a.Id), c);
        if (!sameNetwork) return;
        // Plugged into Thunderbolt with the saved adapter unplugged: the user is moving to the new link.
        if (Net.PickAdapter(list, a.Id) != a) return;

        adapters.SelectedItem = a;
        if (a.Wireless) peer.Text = s[1];
        else
        {
            (c.Ip.EndsWith(".2") ? roleB : roleA).IsChecked = true;
            ip.Text = c.Ip; mask.Text = c.Mask; peer.Text = s[1];
            gateway.Text = c.Gateway; dns.Text = string.Join(",", c.Dns);
        }
        allowedPeer = s[1];
        if (Sys.FirewallRuleExists(Net.RuleName)) StartReceiver();
        rdpToggle.IsChecked = rdpToggle.IsEnabled && Sys.FirewallRuleExists(Net.RdpRuleName);
        Say("已設定完成,可以直接拖檔案傳送。", "Green");
    }

    void Apply()
    {
        var a = adapters.SelectedItem as Adapter;
        if (a == null) { Say("找不到有線網卡", "Red"); return; }

        var c = new Config { Ip = ip.Text.Trim(), Mask = mask.Text.Trim(), Gateway = gateway.Text.Trim(), Dns = Net.ParseDns(dns.Text) };
        string other = peer.Text.Trim();
        // Wi-Fi has no default peer, so this is where the address gets asked for.
        if (a.Wireless && other == "") { OpenAdvanced(); return; }
        string err = Net.Validate(c, other);
        if (err != null) { Say(err, "Red"); return; }

        string conflict = a.Wireless ? null : Sys.SubnetConflict(a.Id, c.Ip, c.Mask);
        if (conflict != null)
        {
            Say(string.Format(Lang.L("「{0}」已經在使用同一個網段,請打開進階設定換一個網段(例如 192.168.252.x)。"), conflict), "Red");
            return;
        }

        bool openFirewall = firewall.IsChecked == true;
        bool allowRdp = rdpToggle.IsChecked == true;
        string rdpNote = allowRdp ? Lang.L(" 對方可以用遠端桌面連進這台。") : "";
        Guard(async () =>
        {
            Say("設定中…請在跳出的視窗按「是」。", "Sub");
            await Task.Run(() =>
            {
                // Already on the target address means the original settings are not here to back up.
                bool unchanged = Net.SameAddress(Sys.ReadConfig(a.Id), c);
                if (!unchanged && !a.Wireless) Sys.BackupOnce(a.Id);
                var steps = Net.AddressSteps(a, c, unchanged);
                steps.Add(Net.Step(false, "netsh", Net.FirewallDelete()));
                steps.Add(Net.Step(false, "net", Net.ShareDelete()));
                steps.AddRange(Net.WebRuleDeleteSteps());
                // ponytail: the Wi-Fi rule stays while on other networks too, One-Click Setup or Restore replaces it; tie it to the SSID if that matters.
                if (openFirewall) steps.AddRange(Net.FirewallCommands(other, a.Wireless).Select(cmd => Net.Step(true, "netsh", cmd)));
                if (allowRdp) Sys.BackupRdpOnce();
                steps.AddRange(allowRdp ? Net.RdpEnableSteps(other, a.Wireless) : Net.RdpDisableSteps(Sys.RdpWasDenied()));
                Sys.RunElevated(steps);
                if (!allowRdp) Sys.DeleteRdpBackup();
                // The service can take a moment after starting, so this allows a few seconds before saying it failed.
                for (int i = 0; allowRdp && i < 5 && !Sys.RdpListening(); i++) Thread.Sleep(1000);
                if (allowRdp && (Sys.RdpDenied() || !Sys.RdpListening())) rdpNote = Lang.L(" 但這台的遠端桌面沒有成功開啟,請檢查 Windows 設定或公司政策。");
                Sys.SaveSession(a.Id, other, c);
            });
            allowedPeer = other;
            StopWeb();
            ShowRdpStatus();
            if (openFirewall)
            {
                // Started only after the rule exists, so Windows doesn't pop its own firewall prompt.
                StartReceiver();
                Say(string.Format(Lang.L("完成!這台是 {0}。另一台也設定好後,把檔案拖到右邊的圖示就能傳送。{1}"), c.Ip, rdpNote), "Green");
            }
            else
            {
                StopReceiver();
                Say(string.Format(Lang.L("完成!這台是 {0}。防火牆未開放,這台只能傳出,收不到對方的檔案。{1}"), c.Ip, rdpNote), "Green");
            }
        });
    }

    void Test()
    {
        string target = peer.Text.Trim();
        if (!Net.IsIp(target)) { Say("對方 IP 格式不正確", "Red"); return; }
        Guard(async () =>
        {
            Say("測試中…", "Sub");
            using (var p = new Ping())
            {
                var r = await p.SendPingAsync(target, 2000);
                if (r.Status == IPStatus.Success)
                    Say(string.Format(Lang.L("連線成功({0} ms)。可以開始傳檔了。"), r.RoundtripTime), "Green");
                else
                    Say("連不到對方。請確認:連接線已接好、兩台分別選 A 和 B、對方也按過「一鍵設定」且防火牆開關為開啟。", "Red");
            }
        });
    }

    async void CheckPeer()
    {
        string target = peer.Text.Trim();
        if (pinging || !Net.IsIp(target)) return;
        pinging = true;
        bool online = false;
        try
        {
            using (var p = new Ping())
                online = (await p.SendPingAsync(target, 1000)).Status == IPStatus.Success;
        }
        catch (PingException) { }
        peerState.Text = Lang.L(online ? "已連線 · " : "未連線 · ") + target;
        peerDot.SetResourceReference(Shape.FillProperty, online ? "Green" : "Sub");
        pinging = false;
    }

    void PickFiles()
    {
        var d = new OpenFileDialog { Multiselect = true, Title = Lang.L("選擇要傳給對方的檔案") };
        if (d.ShowDialog(w) == true) SendFiles(d.FileNames);
    }

    Action<string, long, long> Reporter(string verb, bool canCancel)
    {
        verb = Lang.L(verb);
        var clock = Stopwatch.StartNew();
        long lastMs = -1000;
        w.Dispatcher.BeginInvoke(new Action(() =>
        {
            progress.Value = 0;
            progressText.Text = verb + "…";
            cancelBtn.Visibility = canCancel ? Visibility.Visible : Visibility.Collapsed;
            progressRow.Visibility = Visibility.Visible;
        }));
        return (name, done, total) =>
        {
            long ms = clock.ElapsedMilliseconds;
            if (ms - lastMs < 100) return;
            lastMs = ms;
            double fraction = total > 0 ? (double)done / total : 1;
            double mbps = done / 1048576.0 / Math.Max(ms / 1000.0, 0.001);
            w.Dispatcher.BeginInvoke(new Action(() =>
            {
                progress.Value = fraction;
                progressText.Text = string.Format("{0} {1:P0} · {2:F0} MB/s · {3}", verb, fraction, mbps, name);
            }));
        };
    }

    void HideProgress() { progressRow.Visibility = Visibility.Hidden; }

    void SendFiles(string[] paths)
    {
        string target = peer.Text.Trim(), folder = sendTo.Text.Trim();
        if (!Net.IsIp(target)) { Say("對方 IP 格式不正確", "Red"); return; }
        if (folder != "" && !Net.IsFullFolder(folder)) { Say("請輸入對方電腦上的完整路徑,例如 D:\\收件", "Red"); return; }
        cancelled = false;
        Guard(async () =>
        {
            Say("準備傳送…", "Sub");
            try
            {
                int count = await Task.Run(() =>
                {
                    var items = Transfer.Collect(paths);
                    using (var c = ConnectPeer(target))
                        Transfer.Send(c.GetStream(), items, Reporter("傳送中", true), folder);
                    return items.Count(i => i.Length >= 0);
                });
                Say(string.Format(Lang.L(folder == "" ? "已傳送 {0} 個檔案。" : "已傳送 {0} 個檔案到 {1}。"), count, folder), "Green");
            }
            catch (Exception ex)
            {
                if (cancelled) Say("已取消傳送,對方不會留下未完成的檔案。", "Sub");
                else if (ex is TimeoutException || ex is AggregateException || ex is SocketException)
                    Say("連不到對方。請確認連接線已接好、對方已按「一鍵設定」,且工具保持開啟。", "Red");
                else Say(Lang.L("傳送失敗:") + Lang.L(ex.Message), "Red");
            }
            sendClient = null;
            HideProgress();
        });
    }

    TcpClient ConnectPeer(string target)
    {
        var c = new TcpClient();
        sendClient = c;
        c.SendTimeout = c.ReceiveTimeout = 60000;
        try
        {
            if (!c.ConnectAsync(target, Net.TransferPort).Wait(3000)) throw new TimeoutException();
        }
        catch
        {
            c.Close();
            throw;
        }
        return c;
    }

    // Not ConnectPeer: this can run during a transfer and must not take over the connection the cancel button closes.
    static T AskPeer<T>(string target, Func<Stream, T> ask)
    {
        using (var c = new TcpClient())
        {
            c.SendTimeout = c.ReceiveTimeout = 5000;
            if (!c.ConnectAsync(target, Net.TransferPort).Wait(3000)) throw new TimeoutException();
            return ask(c.GetStream());
        }
    }

    static string PeerFailure(Exception ex)
    {
        if (ex is TimeoutException || ex is AggregateException || ex is SocketException)
            return Lang.L("連不到對方。請確認連接線已接好、對方已按「一鍵設定」,且工具保持開啟。");
        if (ex is IOException) return Lang.L("對方沒有回應,請確認兩台都是新版工具。");
        return Lang.L("無法開啟這個資料夾:") + Lang.L(ex.Message);
    }

    void OpenBrowse()
    {
        if (!Net.IsIp(peer.Text.Trim())) { Say("對方 IP 格式不正確", "Red"); return; }
        // Disabled underneath so Enter can't reach One-Click Setup, the default button.
        F<ScrollViewer>("Scroll").IsEnabled = false;
        F<Border>("BrowseOverlay").Visibility = Visibility.Visible;
        F<StackPanel>("BrowseList").Children.Clear();
        F<Button>("BrowseCancelBtn").Focus();
        string start = sendTo.Text.Trim();
        BrowseTo(Net.IsFullFolder(start) ? start : "");
    }

    void CloseBrowse()
    {
        browseRequest++; // A reply still on its way is ignored.
        F<Border>("BrowseOverlay").Visibility = Visibility.Collapsed;
        F<ScrollViewer>("Scroll").IsEnabled = true;
    }

    async void BrowseTo(string folder)
    {
        int request = ++browseRequest;
        var message = F<TextBlock>("BrowseMessage");
        var up = F<Button>("BrowseUpBtn");
        var select = F<Button>("BrowseSelectBtn");
        up.IsEnabled = select.IsEnabled = false;
        message.Text = "";
        string target = peer.Text.Trim();
        string[] reply = null;
        string failure = null;
        try { reply = await Task.Run(() => AskPeer(target, s => Transfer.PeerFolders(s, folder))); }
        catch (Exception ex) { failure = PeerFailure(ex); }
        if (request != browseRequest) return;

        if (reply != null)
        {
            browsePath = reply[0];
            F<TextBlock>("BrowsePath").Text = browsePath == "" ? Lang.L("對方的磁碟") : browsePath;
            var list = F<StackPanel>("BrowseList");
            list.Children.Clear();
            foreach (var name in reply.Skip(1))
            {
                string next = browsePath == "" ? name : Path.Combine(browsePath, name);
                var item = new Button { Content = name, Style = (Style)w.FindResource("FolderItem") };
                item.Click += (s, e) => BrowseTo(next);
                list.Children.Add(item);
            }
            // Focus stays inside the overlay after the clicked item is gone, so Esc keeps working.
            F<Button>("BrowseCancelBtn").Focus();
            if (list.Children.Count > 0) list.Children[0].Focus();
            else message.Text = Lang.L("這裡沒有子資料夾");
        }
        else message.Text = failure;
        up.IsEnabled = select.IsEnabled = browsePath != "";
    }

    void RunSpeedTest()
    {
        string target = peer.Text.Trim();
        if (!Net.IsIp(target)) { Say("對方 IP 格式不正確", "Red"); return; }
        cancelled = false;
        Guard(async () =>
        {
            Say("測速中,約需 10 秒…", "Sub");
            progress.Value = 0;
            progressText.Text = Lang.L("測速中…");
            cancelBtn.Visibility = Visibility.Visible;
            progressRow.Visibility = Visibility.Visible;
            var clock = Stopwatch.StartNew();
            long lastMs = -1000;
            Action<string, double> report = (phase, fraction) =>
            {
                if (clock.ElapsedMilliseconds - lastMs < 100) return;
                lastMs = clock.ElapsedMilliseconds;
                w.Dispatcher.BeginInvoke(new Action(() =>
                {
                    progress.Value = fraction;
                    progressText.Text = Lang.L("測速中 · ") + Lang.L(phase);
                }));
            };
            try
            {
                long[] r = await Task.Run(() =>
                {
                    using (var c = ConnectPeer(target)) return Transfer.SpeedTest(c.GetStream(), report);
                });
                Say(string.Format(Lang.L("傳給對方 {0}\n從對方接收 {1}"), Net.SpeedText(r[0], r[1]), Net.SpeedText(r[2], r[3])), "Green");
            }
            catch (Exception ex)
            {
                if (cancelled) Say("已取消測速。", "Sub");
                else if (ex is TimeoutException || ex is AggregateException || ex is SocketException)
                    Say("連不到對方。請確認連接線已接好、對方已按「一鍵設定」,且工具保持開啟。", "Red");
                else Say(Lang.L("測速失敗,請確認兩台都是新版工具。") + Lang.L(ex.Message), "Red");
            }
            sendClient = null;
            HideProgress();
        });
    }

    void CancelSend()
    {
        var c = sendClient;
        if (c == null) return;
        cancelled = true;
        c.Close();
    }

    void StartReceiver()
    {
        if (listener != null) return;
        var l = new TcpListener(IPAddress.Any, Net.TransferPort);
        try { l.Start(); }
        catch (SocketException) { return; } // Another window of this app already receives.
        listener = l;
        var thread = new Thread(() => ReceiveLoop(l)) { IsBackground = true };
        thread.Start();
    }

    void StopReceiver()
    {
        if (listener == null) return;
        listener.Stop();
        listener = null;
    }

    void ReceiveLoop(TcpListener l)
    {
        while (true)
        {
            TcpClient c;
            try { c = l.AcceptTcpClient(); }
            catch (SocketException) { return; }
            catch (ObjectDisposedException) { return; }

            using (c)
            {
                // ponytail: one connection at a time; the read timeout keeps a stalled sender from blocking forever.
                if (((IPEndPoint)c.Client.RemoteEndPoint).Address.ToString() != allowedPeer) continue;
                c.SendTimeout = c.ReceiveTimeout = 60000;
                string msg = null, color = "Green", folder = Sys.Inbox;
                receiving = true;
                try
                {
                    int files = Transfer.Receive(c.GetStream(), folder, Reporter("接收中", false), chosen => folder = chosen);
                    if (files == -1) msg = "對方完成測速。";
                    else if (files >= 0) msg = string.Format(Lang.L("收到 {0} 個檔案,存在 {1}。"), files, folder);
                }
                catch (Exception ex)
                {
                    msg = Lang.L("接收中斷,未完成的檔案已清除。") + Lang.L(ex.Message);
                    color = "Red";
                }
                receiving = false;
                // A peer only listing folders leaves the screen alone.
                if (msg != null) w.Dispatcher.BeginInvoke(new Action(() => { Say(msg, color); HideProgress(); }));
            }
        }
    }

    void OpenInbox()
    {
        // The chosen folder can be on a removed drive, so this can fail.
        try
        {
            Directory.CreateDirectory(Sys.Inbox);
            Process.Start(Sys.Inbox);
        }
        catch (Exception ex) { Say(Lang.L("無法開啟收件位置:") + ex.Message, "Red"); }
    }

    void ShowInbox()
    {
        var label = F<TextBlock>("InboxPath");
        label.Text = Sys.Inbox;
        label.ToolTip = Sys.Inbox;
    }

    void PickInbox()
    {
        using (var d = new System.Windows.Forms.FolderBrowserDialog { Description = Lang.L("選擇收到的檔案要存放的資料夾"), SelectedPath = Sys.Inbox })
        {
            if (d.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            try
            {
                Sys.Inbox = d.SelectedPath;
                ShowInbox();
                Say(Lang.L("之後收到的檔案會存到 ") + d.SelectedPath, "Green");
            }
            catch (Exception ex) { Say(Lang.L("無法儲存收件位置:") + ex.Message, "Red"); }
        }
    }

    void ResetInbox()
    {
        try
        {
            Sys.ResetInbox();
            ShowInbox();
            Say(Lang.L("之後收到的檔案會存到 ") + Sys.Inbox, "Green");
        }
        catch (Exception ex) { Say(Lang.L("無法儲存收件位置:") + ex.Message, "Red"); }
    }

    void ConnectRdp()
    {
        string target = peer.Text.Trim();
        if (!Net.IsIp(target)) { Say("對方 IP 格式不正確", "Red"); return; }
        Guard(async () =>
        {
            Say("檢查對方的遠端桌面…", "Sub");
            using (var c = new TcpClient())
            {
                var connect = c.ConnectAsync(target, Net.RdpPort);
                if (await Task.WhenAny(connect, Task.Delay(2000)) != connect || connect.IsFaulted)
                {
                    Say("對方沒有開放遠端桌面。請對方打開「允許對方遠端桌面」再按一次「一鍵設定」(Home 版無法被連線)。", "Red");
                    return;
                }
            }
            Process.Start(Path.Combine(Environment.SystemDirectory, "mstsc.exe"), "/v:" + target);
            Say("已開啟遠端桌面,請輸入對方電腦的帳號密碼。連線期間對方的螢幕會鎖定。", "Green");
        });
    }

    // For a PC left on an old setup, where Restore has no backup to work from.
    void UseAutomaticIp()
    {
        CloseAdvanced(); // It covers the status line.
        var a = adapters.SelectedItem as Adapter;
        if (a == null) { Say("找不到有線網卡", "Red"); return; }
        if (a.Wireless) { Say("Wi-Fi 和熱點不用改,這個按鈕只用於網路線和 Thunderbolt。", "Red"); return; }
        Guard(async () =>
        {
            Say("設定中…如果跳出權限視窗,請按「是」。", "Sub");
            await Task.Run(() =>
            {
                Sys.RunElevated(Net.ConfigSteps(a.Name, new Config { Dhcp = true }, false));
                if (!Sys.ReadConfig(a.Id).Dhcp) throw new Exception("無法改成自動取得 IP,請再試一次。");
                // Otherwise Restore, or quitting, would put the old address back.
                Sys.DeleteBackup(a.Id);
            });
            var session = Sys.LoadSession();
            if (session != null && session[0] == a.Id)
            {
                Sys.DeleteSession();
                StopReceiver();
                allowedPeer = null;
            }
            Say("已改成自動取得 IP。", "Green");
        });
    }

    void Restore() { Guard(RestoreWork); }

    // Every adapter with a backup, not just the selected one: a PC can have both the cable and Thunderbolt set up.
    async Task RestoreWork()
    {
        var changed = ((List<Adapter>)adapters.ItemsSource).Where(x => Sys.LoadBackup(x.Id) != null).ToList();
        Say("還原中…請在跳出的視窗按「是」。", "Sub");
        string result = await Task.Run(() =>
        {
            var steps = new List<string> { Net.Step(false, "netsh", Net.FirewallDelete()), Net.Step(false, "net", Net.ShareDelete()) };
            steps.AddRange(Net.WebRuleDeleteSteps());
            foreach (var a in changed) steps.AddRange(Net.ConfigSteps(a.Name, Sys.LoadBackup(a.Id), false));
            steps.AddRange(Net.RdpDisableSteps(Sys.RdpWasDenied()));
            Sys.RunElevated(steps);
            Sys.DeleteRdpBackup();
            Sys.DeleteSession();
            if (changed.Count == 0) return "已移除防火牆規則。這張網卡沒有備份,IP 未變更。";

            foreach (var a in changed)
            {
                if (Sys.ReadConfig(a.Id).Dhcp != Sys.LoadBackup(a.Id).Dhcp) throw new Exception("還原 IP 失敗,備份已保留,請再試一次。");
                Sys.DeleteBackup(a.Id);
            }
            return "已還原成原本的網路設定,並移除防火牆規則。";
        });
        StopReceiver();
        StopWeb();
        allowedPeer = null;
        rdpToggle.IsChecked = false;
        Say(result, "Green");
        if (hotspotToggle.IsChecked == true) await RunHotspot("stop");
    }
}

static class Program
{
    public const string ShowSignalName = "Local\\LanLink.Show";

    // Tells the old copy this one runs, then waits for it to leave so this one can take the single-instance lock.
    static string[] FinishUpdate(string oldProcess, string mode)
    {
        try
        {
            using (var started = EventWaitHandle.OpenExisting(Updater.StartedSignalName)) started.Set();
            using (var old = Process.GetProcessById(int.Parse(oldProcess))) old.WaitForExit(15000);
        }
        catch (Exception) { } // The old copy is already gone.
        return mode == "tray" ? new[] { "--tray" } : new string[0];
    }

    [STAThread]
    static int Main(string[] args)
    {
        if (args.Length > 0 && args[0] == "--selftest") return Net.SelfTest();
        Lang.Load();
        if (args.Length == 3 && args[0] == "--admin") return Sys.RunSteps(args[1], args[2]);
        if (args.Length == 3 && args[0] == "--after-update") args = FinishUpdate(args[1], args[2]);
        bool hidden = args.Length == 1 && args[0] == "--tray";
        bool first;
        using (new Mutex(true, "Local\\LanLink.Running", out first))
        {
            if (first) Updater.Cleanup(Sys.ProgramPath);
            // One copy keeps the receive port and tray icon. A plain launch brings it up; a startup launch has nothing to do.
            if (!first && hidden) return 0;
            if (!first && args.Length == 0)
            {
                using (var signal = new EventWaitHandle(false, EventResetMode.AutoReset, ShowSignalName)) signal.Set();
                return 0;
            }
            // Any other arguments are files dropped on the exe or its shortcut.
            new Ui(hidden ? new string[0] : args, first).Run(hidden);
        }
        return 0;
    }
}
