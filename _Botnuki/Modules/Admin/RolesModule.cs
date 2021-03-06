﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Text;

namespace _Botnuki.Modules.Admin
{
    class RolesModule : ModuleBase
    {
        string[] AuthorizedRoles = { "league", "smite", "paladins", "battlerite", "ark" };

        // methods
        [Command("roleadd"), Alias("addrole", "ra", "ar", "addroles", "rolesadd")]
        [Summary("Allow a user to add a role or multiple roles to themself")]
        public async Task RoleAdd(params string[] roles)
        {
            try
            {
                var chan = Context.Channel;
                var userinfo = Context.User as SocketGuildUser;
                var g = Context.Guild;
                var userRoleIDs = userinfo.Roles.ToList(); //.RoleIds.ToList();
                List<IRole> RolesAuthed = new List<IRole>();

                foreach (string r in roles)
                {
                    var ro = GetRole(g, r.ToLower());
                    if (!(AuthorizedRoles.Contains(ro.Name.ToLower())))
                    {
                        await ReplyAsync($"The role you tried to add either doesn't exist or is not a role you are authorized for.");
                    }
                    else
                    {
                        RolesAuthed.Add(ro);
                    }
                }

                if (RolesAuthed.Count() > 0)
                {
                    IRole[] r = RolesAuthed.ToArray();
                    await userinfo.AddRolesAsync(r);
                    await ReplyAsync("The authorized roles have been added!");
                }

            }
            catch (Exception ex)
            {
                await ReplyAsync("The role you have tried to add is not authorized.");
                
                Console.WriteLine(ErrorHandling.ThrowGenException("RolesModule.cs", "RoleAdd", ex.Message));
            }

        }

        [Command("roleremove"), Alias("rolerem", "removerole", "remrole", "rr", "rrem", "rolesremove", "rolesrem", "removeroles", "remroles")]
        [Summary("Allow a user to remove a role from themself, or another user if allowed")]
        public async Task RoleRemove(params string[] roles)
        {
            try
            {
                var chan = Context.Channel;
                var userinfo = Context.User as SocketGuildUser;
                var g = Context.Guild;
                

                var userRoleIDs = userinfo.Roles.ToList();
                List<IRole> RolesAuthed = new List<IRole>();

                foreach (string r in roles)
                {
                    var ro = GetRole(g, r.ToLower());
                    if (!(AuthorizedRoles.Contains(ro.Name.ToLower())))
                    {
                        await ReplyAsync($"The role you tried to remove either doesn't exist or is not a role you are authorized for.");
                    }
                    else
                    {
                        RolesAuthed.Add(ro);
                    }
                }

                if (RolesAuthed.Count() > 0)
                {
                    IRole[] r = RolesAuthed.ToArray();
                    await userinfo.RemoveRolesAsync(r);
                    await ReplyAsync("The authorized roles have been removed!");
                }

            }
            catch (Exception ex)
            {
                await ReplyAsync("The role you have tried to remove is not authorized.");
                Console.WriteLine(ErrorHandling.ThrowGenException("RolesModule.cs", "RoleRemove", ex.Message));
            }
        }

        [Command("roles"), Summary("A help command explaining roles authorized")]
        public async Task Roles()
        {
            StringBuilder str = new StringBuilder();
            str.Append($"You are allowed to use me to add the following roles to your user profile:\n");

            foreach(string s in AuthorizedRoles)
            {
                str.Append($"• {s}\n");
            }
            await ReplyAsync(str.ToString());
        }

        [Command("mute"), RequireContext(ContextType.Guild)]
        public async Task Mute(IGuildUser u)
        {
            try
            {
                // first, make sure the user executing this command has permission
                if (RoleSearch((SocketGuildUser)Context.User, Context.Guild) == false)
                {
                    await ReplyAsync("You do not have permission to use this command!");
                    return;
                }
                string[] role = { "silenced" };
                // mute the user
                var silence = GetRole(Context.Guild, role[0]);
                await u.AddRolesAsync(silence);

                await ReplyAsync($"User {u.Nickname} has been muted!");

            }
            catch (Exception ex)
            {
                await ReplyAsync("You do not have permission to use this command!");
                Console.WriteLine(ErrorHandling.ThrowGenException("RolesModule", "Mute", ex.Message));
            }
        }

        [Command("unmute"), RequireContext(ContextType.Guild)]
        public async Task Unmute(IGuildUser u)
        {
            try
            {
                // first, make sure the user executing this command has permission
                if (RoleSearch((SocketGuildUser)Context.User, Context.Guild) == false)
                {
                    await ReplyAsync("You do not have permission to use this command!");
                    return;
                }
                string[] role = { "silenced" };
                // unmute the user
                var unsilence = GetRole(Context.Guild, role[0]);
                await u.RemoveRolesAsync(unsilence);

                await ReplyAsync($"User {u.Nickname} has been unmuted!");

            }
            catch (Exception ex)
            {
                await ReplyAsync("You do not have permission to use this command!");
                Console.WriteLine(ErrorHandling.ThrowGenException("RolesModule", "Unmute", ex.Message));
            }
        }

        public IRole GetRole(IGuild g, ulong roleID)
            =>
            (from role in g.Roles
             where role.Id.Equals(roleID)
             select g.GetRole(role.Id)).FirstOrDefault();

        public IRole GetRole(IGuild g, string roleContains)
            =>
            (from role in g.Roles
             where role.Name.ToLower().Contains(roleContains)
             select g.GetRole(role.Id)).FirstOrDefault();

        public bool RoleSearch(SocketGuildUser u, IGuild g)
        {
            var chan = Context.Channel;
            try
            {
                string[] ValidRoles = { "owners", "manager", "moderators" };
                var userRoles = u.Roles.ToList();
                int i, count = userRoles.Count();

                for (i = 0; i < count; i++)
                {
                    ulong userRoleID = userRoles[i].Id;
                    var roleComp = GetRole(g, userRoleID);

                    for (int j = 0; j < ValidRoles.Count(); j++)
                    {
                        var role = GetRole(g, ValidRoles[j]);
                        if (role.Name == roleComp.Name) return true;
                        else continue;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Error when attempting RoleSearch. {ex.Message}");
                return false;
            }
        }
    }
}
