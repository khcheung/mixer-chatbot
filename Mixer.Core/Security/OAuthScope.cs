using System;
using System.Collections.Generic;
using System.Text;

namespace Mixer.Core.Security
{
    /// <summary>
    /// OAuth Scope
    /// https://dev.mixer.com/reference/oauth/scopes
    /// </summary>
    public enum OAuthScope
    {
        user__act_as,
        achievement__view__self,
        channel__analytics__self,
        channel__clip__create__self,
        channel__clip__delete__self,
        channel__costream__self,
        channel__deleteBanner__self,
        channel__details__self,
        channel__follow__self,
        channel__streamKey__self,
        channel__teststream__view__self,
        channel__update__self,
        chat__bypass_catbot,
        chat__bypass_filter,
        chat__bypass_links,
        chat__bypass_slowchat,
        chat__cancel_skill,
        chat__change_ban,
        chat__change_role,
        chat__chat,
        chat__clear_messages,
        chat__connect,
        chat__edit_options,
        chat__giveaway_start,
        chat__poll_start,
        chat__poll_vote,
        chat__purge,
        chat__remove_message,
        chat__timeout,
        chat__view_deleted,
        chat__whisper,
        delve__view__self,
        interactive__manage__self,
        interactive__robot__self,
        invoice__view__self,
        log__view__self,
        oauth__manage__self,
        recording__manage__self,
        redeemable__create__self,
        redeemable__redeem__self,
        redeemable__view__self,
        resource__find__self,
        subscription__cancel__self,
        subscription__create__self,
        subscription__renew__self,
        subscription__view__self,
        team__manage__self,
        transaction__cancel__self,
        transaction__view__self,
        user__analytics__self,
        user__details__self,
        user__getDiscordInvite__self,
        user__log__self,
        user__notification__self,
        user__seen__self,
        user__update__self,
        user__updatePassword__self,

    }
}
