using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace MiView.Common.AnalyzeData.Format.Misskey.v2025.API
{
    public class Meta
    {
        private readonly JsonNode? Node;
        public Meta(JsonNode? node) { Node = node; }

        public JsonNode? MaintainerName => Node?["maintainerName"];
        public JsonNode? MaintainerEmail => Node?["maintainerEmail"];
        public JsonNode? Version => Node?["version"];
        public JsonNode? Name => Node?["name"];
        public JsonNode? ShortName => Node?["shortName"];
        public JsonNode? Uri => Node?["uri"];
        public JsonNode? Description => Node?["description"];
        public JsonNode? Langs => Node?["langs"];
        public JsonNode? TosUrl => Node?["tosUrl"];
        public JsonNode? RepositoryUrl => Node?["repositoryUrl"];
        public JsonNode? FeedbackUrl => Node?["feedbackUrl"];
        public JsonNode? DefaultDarkTheme => Node?["defaultDarkTheme"];
        public JsonNode? DefaultLightTheme => Node?["defaultLightTheme"];
        public JsonNode? DisableRegistration => Node?["disableRegistration"];
        public JsonNode? EmailRequiredForSignup => Node?["emailRequiredForSignup"];
        public JsonNode? EnableHcaptcha => Node?["enableHcaptcha"];
        public JsonNode? HcaptchaSiteKey => Node?["hcaptchaSiteKey"];
        public JsonNode? EnableMcaptcha => Node?["enableMcaptcha"];
        public JsonNode? McaptchaSiteKey => Node?["mcaptchaSiteKey"];
        public JsonNode? McaptchaInstanceUrl => Node?["mcaptchaInstanceUrl"];
        public JsonNode? EnableRecaptcha => Node?["enableRecaptcha"];
        public JsonNode? RecaptchaSiteKey => Node?["recaptchaSiteKey"];
        public JsonNode? EnableTurnstile => Node?["enableTurnstile"];
        public JsonNode? TurnstileSiteKey => Node?["turnstileSiteKey"];
        public JsonNode? GoogleAnalyticsId => Node?["googleAnalyticsId"];
        public JsonNode? SwPublickey => Node?["swPublickey"];
        public JsonNode? MascotImageUrl => Node?["mascotImageUrl"];
        public JsonNode? BannerUrl => Node?["bannerUrl"];
        public JsonNode? ServerErrorImageUrl => Node?["serverErrorImageUrl"];
        public JsonNode? InfoImageUrl => Node?["infoImageUrl"];
        public JsonNode? NotFoundImageUrl => Node?["notFoundImageUrl"];
        public JsonNode? IconUrl => Node?["iconUrl"];
        public JsonNode? MaxNoteTextLength => Node?["maxNoteTextLength"];
        public JsonNode? WellKnownWebsites => Node?["wellKnownWebsites"];
        public JsonNode? NotesPerOneAd => Node?["notesPerOneAd"];
        public JsonNode? EnableEmail => Node?["enableEmail"];
        public JsonNode? EnableServiceWorker => Node?["enableServiceWorker"];
        public JsonNode? TranslatorAvailable => Node?["translatorAvailable"];
        public JsonNode? MediaProxy => Node?["mediaProxy"];
        public JsonNode? EnableUrlPreview => Node?["enableUrlPreview"];
        public JsonNode? EnableSkebStatus => Node?["enableSkebStatus"];
        public JsonNode? BackgroundImageUrl => Node?["backgroundImageUrl"];
        public JsonNode? ImpressumUrl => Node?["impressumUrl"];
        public JsonNode? LogoImageUrl => Node?["logoImageUrl"];
        public JsonNode? PrivacyPolicyUrl => Node?["privacyPolicyUrl"];
        public JsonNode? ServerRules => Node?["serverRules"];
        public JsonNode? ThemeColor => Node?["themeColor"];
        public JsonNode? ProxyAccountName => Node?["proxyAccountName"];
        public JsonNode? RequireSetup => Node?["requireSetup"];
        public JsonNode? CacheRemoteFiles => Node?["cacheRemoteFiles"];
        public JsonNode? CacheRemoteSensitiveFiles => Node?["cacheRemoteSensitiveFiles"];

        // --- 子クラス ---
        public Policies? Policies => Node?["policies"] is JsonNode n ? new Policies(n) : null;
        public Features? Features => Node?["features"] is JsonNode n ? new Features(n) : null;
        public JsonArray? Ads => Node?["ads"] as JsonArray;
    }

    // -------------------- //
    // --- Policies class --- //
    // -------------------- //

    public class Policies
    {
        private readonly JsonNode? Node;
        public Policies(JsonNode? node) { Node = node; }

        public JsonNode? GtlAvailable => Node?["gtlAvailable"];
        public JsonNode? LtlAvailable => Node?["ltlAvailable"];
        public JsonNode? CanPublicNote => Node?["canPublicNote"];
        public JsonNode? CanScheduleNote => Node?["canScheduleNote"];
        public JsonNode? ScheduleNoteLimit => Node?["scheduleNoteLimit"];
        public JsonNode? ScheduleNoteMaxDays => Node?["scheduleNoteMaxDays"];
        public JsonNode? CanInitiateConversation => Node?["canInitiateConversation"];
        public JsonNode? CanCreateContent => Node?["canCreateContent"];
        public JsonNode? CanUpdateContent => Node?["canUpdateContent"];
        public JsonNode? CanDeleteContent => Node?["canDeleteContent"];
        public JsonNode? CanPurgeAccount => Node?["canPurgeAccount"];
        public JsonNode? CanUpdateAvatar => Node?["canUpdateAvatar"];
        public JsonNode? CanUpdateBanner => Node?["canUpdateBanner"];
        public JsonNode? MentionLimit => Node?["mentionLimit"];
        public JsonNode? CanInvite => Node?["canInvite"];
        public JsonNode? InviteLimit => Node?["inviteLimit"];
        public JsonNode? InviteLimitCycle => Node?["inviteLimitCycle"];
        public JsonNode? InviteExpirationTime => Node?["inviteExpirationTime"];
        public JsonNode? CanManageCustomEmojis => Node?["canManageCustomEmojis"];
        public JsonNode? CanManageAvatarDecorations => Node?["canManageAvatarDecorations"];
        public JsonNode? CanSearchNotes => Node?["canSearchNotes"];
        public JsonNode? CanUseTranslator => Node?["canUseTranslator"];
        public JsonNode? CanUseDriveFileInSoundSettings => Node?["canUseDriveFileInSoundSettings"];
        public JsonNode? CanUseReaction => Node?["canUseReaction"];
        public JsonNode? CanHideAds => Node?["canHideAds"];
        public JsonNode? DriveCapacityMb => Node?["driveCapacityMb"];
        public JsonNode? AlwaysMarkNsfw => Node?["alwaysMarkNsfw"];
        public JsonNode? SkipNsfwDetection => Node?["skipNsfwDetection"];
        public JsonNode? PinLimit => Node?["pinLimit"];
        public JsonNode? AntennaLimit => Node?["antennaLimit"];
        public JsonNode? AntennaNotesLimit => Node?["antennaNotesLimit"];
        public JsonNode? WordMuteLimit => Node?["wordMuteLimit"];
        public JsonNode? WebhookLimit => Node?["webhookLimit"];
        public JsonNode? ClipLimit => Node?["clipLimit"];
        public JsonNode? NoteEachClipsLimit => Node?["noteEachClipsLimit"];
        public JsonNode? UserListLimit => Node?["userListLimit"];
        public JsonNode? UserEachUserListsLimit => Node?["userEachUserListsLimit"];
        public JsonNode? RateLimitFactor => Node?["rateLimitFactor"];
        public JsonNode? AvatarDecorationLimit => Node?["avatarDecorationLimit"];
        public JsonNode? MutualLinkSectionLimit => Node?["mutualLinkSectionLimit"];
        public JsonNode? MutualLinkLimit => Node?["mutualLinkLimit"];
    }

    // -------------------- //
    // --- Features class --- //
    // -------------------- //

    public class Features
    {
        private readonly JsonNode? Node;
        public Features(JsonNode? node) { Node = node; }

        public JsonNode? Registration => Node?["registration"];
        public JsonNode? EmailRequiredForSignup => Node?["emailRequiredForSignup"];
        public JsonNode? LocalTimeline => Node?["localTimeline"];
        public JsonNode? GlobalTimeline => Node?["globalTimeline"];
        public JsonNode? HCaptcha => Node?["hCaptcha"];
        public JsonNode? Hcaptcha => Node?["hcaptcha"];
        public JsonNode? MCaptcha => Node?["mCaptcha"];
        public JsonNode? Mcaptcha => Node?["mcaptcha"];
        public JsonNode? ReCaptcha => Node?["reCaptcha"];
        public JsonNode? Recaptcha => Node?["recaptcha"];
        public JsonNode? Turnstile => Node?["turnstile"];
        public JsonNode? ObjectStorage => Node?["objectStorage"];
        public JsonNode? ServiceWorker => Node?["serviceWorker"];
        public JsonNode? Miauth => Node?["miauth"];
    }

    // -------------------- //
    // --- Ad class --- //
    // -------------------- //

    public class Ad
    {
        private readonly JsonNode? Node;
        public Ad(JsonNode? node) { Node = node; }

        public JsonNode? Id => Node?["id"];
        public JsonNode? Url => Node?["url"];
        public JsonNode? Place => Node?["place"];
        public JsonNode? Ratio => Node?["ratio"];
        public JsonNode? ImageUrl => Node?["imageUrl"];
        public JsonNode? DayOfWeek => Node?["dayOfWeek"];
    }
}
