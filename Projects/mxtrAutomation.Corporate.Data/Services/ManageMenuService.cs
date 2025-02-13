using System;
using System.Collections.Generic;
using System.Linq;
using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Corporate.Data.Entities;
using mxtrAutomation.Data.Repository;
using mxtrAutomation.Corporate.Data.Enums;

namespace mxtrAutomation.Corporate.Data.Services
{
    public class ManageMenuService : MongoRepository<MenuCustomization>, IManageMenuServiceInternal
    {
        #region Customize menu
        public List<ManageMenuDataModel> GetMenuData(string accountObjectID)
        {
            List<ManageMenuDataModel> lstMenuData = new List<ManageMenuDataModel>();
            using (MongoRepository<MenuCustomization> repo = new MongoRepository<MenuCustomization>())
            {
                var data = repo.Where(x => x.AccountObjectID == accountObjectID).ToList();
                foreach (var item in data)
                {
                    lstMenuData.Add(new ManageMenuDataModel
                    {
                        AccountObjectID = item.AccountObjectID,
                        MenuID = item.Id,
                        MenuMasterId = item.MenuMasterId,
                        Name = item.Name,
                        //MenuIdentifier = item.MenuIdentifier,
                        //PageUrl = item.PageUrl,
                        Icon = item.Icon,
                        SortOrder = item.SortOrder,
                        LastModifiedBy = item.LastModifiedBy,
                        SubMenu = CastToSubMenuData(item.SubMenu) != null ? CastToSubMenuData(item.SubMenu).OrderBy(y => y.SortOrder).ToList() : CastToSubMenuData(item.SubMenu),
                    });
                }
            }
            GetMasterMenuName(lstMenuData);
            return lstMenuData.OrderBy(o => o.SortOrder).ToList();
        }

        private static void GetMasterMenuName(List<ManageMenuDataModel> lstMenuData)
        {
            List<string> lstMenuMasterId = lstMenuData.Select(x => x.MenuMasterId).ToList();
            if (lstMenuMasterId != null && lstMenuMasterId.Count() > 0)
            {
                using (MongoRepository<MenuMaster> repoMenuMaster = new MongoRepository<MenuMaster>())
                {
                    List<MenuMaster> menus = repoMenuMaster.Where(r => lstMenuMasterId.Contains(r.Id)).ToList();
                    AssignMenuMasterName(lstMenuData, menus);
                }
            }
        }

        private static void AssignMenuMasterName(List<ManageMenuDataModel> lstMenuData, List<MenuMaster> menus)
        {
            foreach (var menu in lstMenuData)
            {
                var menuData = menus.FirstOrDefault(w => w.Id == menu.MenuMasterId);
                if (menuData != null)
                {
                    menu.MasterMenuName = menuData.Name;
                    menu.PageUrl = menuData.PageUrl;
                    menu.MenuIdentifier = String.IsNullOrEmpty(menuData.MenuIdentifier) ? menuData.Name + Guid.NewGuid().ToString() : menuData.MenuIdentifier;
                    if (menuData.SubMenu != null && menu.SubMenu != null)
                    {
                        foreach (var item in menu.SubMenu)
                        {
                            var subMenuData = menuData.SubMenu.FirstOrDefault(x => x.Id == item.MenuMasterId);
                            if (subMenuData != null)
                            {
                                menu.SubMenu[menu.SubMenu.IndexOf(item)].MasterMenuName = subMenuData.Name;
                                menu.SubMenu[menu.SubMenu.IndexOf(item)].PageUrl = subMenuData.PageUrl;
                                menu.SubMenu[menu.SubMenu.IndexOf(item)].MenuIdentifier = String.IsNullOrEmpty(subMenuData.MenuIdentifier) ? subMenuData.Name + Guid.NewGuid().ToString() : subMenuData.MenuIdentifier;
                            }
                        }
                    }
                }
            }
        }

        public CreateNotificationReturn UpdateMenuData(List<ManageMenuDataModel> menuData)
        {
            try
            {
                using (MongoRepository<MenuCustomization> repo = new MongoRepository<MenuCustomization>())
                {
                    foreach (var item in menuData)
                    {
                        var entry = repo.FirstOrDefault(x => x.AccountObjectID == item.AccountObjectID && x.Id == item.MenuID && x.MenuMasterId == item.MenuMasterId);
                        if (entry == null)
                        {
                            entry = new MenuCustomization();
                            entry.MenuMasterId = item.MenuMasterId;
                            entry.AccountObjectID = item.AccountObjectID;
                            entry.Name = item.Name;
                            //entry.MenuIdentifier = item.MenuIdentifier;
                            //entry.PageUrl = item.PageUrl;
                            entry.Icon = item.Icon;
                            entry.SortOrder = item.SortOrder;
                            entry.LastModifiedBy = item.LastModifiedBy;
                            entry.SubMenu = CastToSubMenuEntity(item.SubMenu);

                            repo.Add(entry);
                        }
                        else
                        {
                            entry.Name = item.Name;
                            entry.Icon = item.Icon;
                            entry.SortOrder = item.SortOrder;
                            entry.LastModifiedBy = item.LastModifiedBy;
                            entry.SubMenu = CastToSubMenuEntity(item.SubMenu);

                            repo.Update(entry);
                        }
                    }
                }

                return new CreateNotificationReturn { Success = true, ObjectID = string.Empty };
            }
            catch (Exception ex)
            {
                return new CreateNotificationReturn { Success = false, ObjectID = string.Empty };
            }
        }

        public CreateNotificationReturn ResetMenuData(string accountObjectID, string userObjectId)
        {
            try
            {
                using (MongoRepository<MenuCustomization> repo = new MongoRepository<MenuCustomization>())
                {
                    var data = repo.Where(x => x.AccountObjectID == accountObjectID).ToList();
                    if (data != null && data.Count > 0)
                    {
                        foreach (var item in data)
                        {
                            repo.Delete(item);
                        }
                    }
                }

                return new CreateNotificationReturn { Success = true, ObjectID = string.Empty };
            }
            catch (Exception e)
            {
                return new CreateNotificationReturn { Success = false, ObjectID = string.Empty };
            }
        }

        #endregion

        #region Default Menu Data
        public bool IsMenuMasterExist()
        {
            using (MongoRepository<MenuMaster> repo = new MongoRepository<MenuMaster>())
            {
                if (repo.Count() > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public void AddMenuMaster()
        {
            List<ManageMenuDataModel> lstMenuData = DefaultMenuList();
            using (MongoRepository<MenuMaster> repo = new MongoRepository<MenuMaster>())
            {
                foreach (var item in lstMenuData)
                {
                    MenuMaster entity = new MenuMaster
                    {
                        Name = item.Name,
                        MenuIdentifier = item.MenuIdentifier,
                        Icon = item.Icon,
                        PageUrl = item.PageUrl,
                        SortOrder = item.SortOrder,
                        Status = item.Status,
                        LastModifiedBy = item.LastModifiedBy,
                        CreatedDate = item.CreatedDate,
                        MenuScope = MenuScopeKind.Global.ToString(),
                        SubMenu = CastToSubMenuMasterEntity(item.SubMenu),
                    };
                    repo.Add(entity);
                }
            }
        }

        private List<ManageMenuDataModel> DefaultMenuList()
        {
            List<ManageMenuDataModel> lstMenu = new List<ManageMenuDataModel>();
            lstMenu.Add(new ManageMenuDataModel
            {
                Name = "Home",
                MenuIdentifier = "home-url",
                PageUrl = "/Index",
                SortOrder = 1,
                Icon = "fa fa-home",
                Status = true,
                CreatedDate = DateTime.UtcNow,
            });

            lstMenu.Add(new ManageMenuDataModel
            {
                Name = "Retailer Performance",
                MenuIdentifier = "retailers-url",
                PageUrl = "/retailers/retailers",
                SortOrder = 2,
                Icon = "fa fa-bar-chart-o",
                Status = true,
                CreatedDate = DateTime.UtcNow,
            });

            lstMenu.Add(new ManageMenuDataModel
            {
                Name = "Contacts",
                MenuIdentifier = "contacts-url",
                PageUrl = "/leads",
                SortOrder = 3,
                Icon = "fa fa-address-book",
                Status = true,
                CreatedDate = DateTime.UtcNow,
            });

            lstMenu.Add(new ManageMenuDataModel
            {
                Name = "Email",
                MenuIdentifier = "email-url",
                PageUrl = "",
                SortOrder = 4,
                Icon = "fa fa-envelope",
                Status = true,
                CreatedDate = DateTime.UtcNow,
                SubMenu = DefaultSubMenus(),
            });

            return lstMenu;
        }

        private List<ManageMenuDataModel> DefaultSubMenus()
        {
            List<ManageMenuDataModel> lstSubMenu = new List<ManageMenuDataModel>();
            lstSubMenu.Add(new ManageMenuDataModel
            {
                Name = "Nurturing Report",
                MenuIdentifier = "liNurturingReport",
                PageUrl = "/email",
                SortOrder = 1,
                Icon = "",
                Status = true,
                CreatedDate = DateTime.UtcNow,
                //SubMenu = SubMenus(),
            });

            return lstSubMenu;
        }

        #endregion

        #region Menu Management

        public List<ManageMenuDataModel> GetMenuMaster()
        {
            List<ManageMenuDataModel> lstMenuData = new List<ManageMenuDataModel>();
            using (MongoRepository<MenuMaster> repo = new MongoRepository<MenuMaster>())
            {
                var data = repo.Where(x => x.Status && (x.MenuScope == MenuScopeKind.Global.ToString() || String.IsNullOrEmpty(x.MenuScope))).ToList();
                foreach (var item in data)
                {
                    lstMenuData.Add(new ManageMenuDataModel
                    {
                        MenuID = item.Id,
                        MenuMasterId = item.Id,
                        Name = item.Name,
                        MasterMenuName = item.Name,
                        MenuIdentifier = item.MenuIdentifier,
                        PageUrl = item.PageUrl,
                        Icon = item.Icon,
                        SortOrder = item.SortOrder,
                        LastModifiedBy = item.LastModifiedBy,
                        MenuScope = item.MenuScope == null ? MenuScopeKind.Global.ToString() : item.MenuScope,
                        SubMenu = CastToSubMenuData(item.SubMenu),
                    });
                }
            }
            return lstMenuData;
        }

        public List<ManageMenuDataModel> GetAllMenuMaster()
        {
            List<ManageMenuDataModel> lstMenuData = new List<ManageMenuDataModel>();
            using (MongoRepository<MenuMaster> repo = new MongoRepository<MenuMaster>())
            {
                var data = repo.Where(x => x.Status).ToList();
                foreach (var item in data)
                {
                    lstMenuData.Add(new ManageMenuDataModel
                    {
                        MenuID = item.Id,
                        MenuMasterId = item.Id,
                        Name = item.Name,
                        MasterMenuName = item.Name,
                        MenuIdentifier = item.MenuIdentifier,
                        PageUrl = item.PageUrl,
                        Icon = item.Icon,
                        SortOrder = item.SortOrder,
                        LastModifiedBy = item.LastModifiedBy,
                        MenuScope = item.MenuScope == null ? MenuScopeKind.Global.ToString() : item.MenuScope,
                        OrganizationAccounts = item.MenuScope == MenuScopeKind.OrganizationBased.ToString() ? GetOrganization(item.Id) : null,
                        SubMenu = CastToSubMenuData(item.SubMenu),
                    });
                }
            }
            return lstMenuData;
        }

        public CreateNotificationReturn AddUpdateMenuMaster(ManageMenuDataModel menuData)
        {
            try
            {
                using (MongoRepository<MenuMaster> repo = new MongoRepository<MenuMaster>())
                {
                    var data = repo.Where(x => x.Id == menuData.MenuID).FirstOrDefault();
                    var previousmenuscope = string.Empty;

                    MenuMaster entity = new MenuMaster
                    {
                        Name = menuData.Name,
                        MenuIdentifier = menuData.MenuIdentifier,
                        Icon = menuData.Icon,
                        PageUrl = menuData.PageUrl,
                        SortOrder = menuData.SortOrder,
                        MenuScope = menuData.MenuScope,
                        Status = menuData.Status,
                        LastModifiedBy = menuData.LastModifiedBy,
                        SubMenu = CastToSubMenuMasterEntity(menuData.SubMenu),
                    };

                    if (data == null)
                    {
                        entity.SortOrder = repo.Max(x => x.SortOrder) + 1;
                        entity.CreatedDate = DateTime.UtcNow;
                        if (entity.SubMenu != null && entity.SubMenu.Count > 0)
                        {
                            for (int i = 0; i < entity.SubMenu.Count; i++)
                            {
                                entity.SubMenu[i].SortOrder = i + 1;
                            }
                        }

                        repo.Add(entity);

                        // Add same menu in customize table
                        if (menuData.MenuScope == MenuScopeKind.Global.ToString())
                        {
                            AddNewGolbalMasterMenu(menuData, entity);
                        }
                        else
                        {
                            AddNewOrgnaizationMasterMenu(menuData, entity);
                        }
                    }
                    else
                    {
                        previousmenuscope = data.MenuScope;
                        data.Name = menuData.Name;
                        data.MenuIdentifier = menuData.MenuIdentifier;
                        data.Icon = menuData.Icon;
                        data.PageUrl = menuData.PageUrl;
                        data.SortOrder = menuData.SortOrder;
                        data.MenuScope = menuData.MenuScope;
                        data.Status = menuData.Status;
                        data.LastModifiedBy = menuData.LastModifiedBy;
                        data.ModifiedDate = DateTime.UtcNow;
                        data.SubMenu = CastToSubMenuMasterEntity(menuData.SubMenu);

                        if (data.SubMenu != null && data.SubMenu.Count > 0)
                        {
                            for (int i = 0; i < data.SubMenu.Count; i++)
                            {
                                data.SubMenu[i].SortOrder = i + 1;
                            }
                        }

                        repo.Update(data);

                        // Update submenu in customize table
                        //UpdateMasterMenuToExistingAccounts(data);

                        if (menuData.MenuScope == MenuScopeKind.Global.ToString() && previousmenuscope == MenuScopeKind.Global.ToString()) // menu scoped not changed
                        {
                            SetMasterMenuToExistingAccounts(data);
                        }
                        else if (menuData.MenuScope == MenuScopeKind.Global.ToString()) //menu scope changed from organization to global
                        {
                            SetMasterMenuToGlobal(data);
                        }
                        else // Handle Organization based
                        {

                            SetMasterMenuToOrganizationBased(menuData, data);
                        }
                    }

                    return new CreateNotificationReturn { Success = true, ObjectID = entity.Id };
                }
            }
            catch (Exception ex)
            {

                return new CreateNotificationReturn { Success = false, ObjectID = string.Empty };
            }
        }

        private void AddNewGolbalMasterMenu(ManageMenuDataModel menuData, MenuMaster entity)
        {
            using (MongoRepository<MenuCustomization> menurepo = new MongoRepository<MenuCustomization>())
            {
                List<ManageMenuDataModel> lstMenuData = new List<ManageMenuDataModel>();
                var repodata = menurepo.ToList();
                var menuAccounts = repodata.GroupBy(info => info.AccountObjectID)
                 .Select(group => new
                 {
                     accountobjectid = group.Key,
                     Count = group.Count()
                 }).ToList();

                if (menuAccounts != null && menuAccounts.Count() > 0)
                {
                    foreach (var item in menuAccounts)
                    {
                        ManageMenuDataModel menuEntity = new ManageMenuDataModel();

                        menuEntity.AccountObjectID = item.accountobjectid;
                        menuEntity.MenuMasterId = entity.Id;

                        menuEntity.Name = menuData.Name;
                        menuEntity.MenuIdentifier = menuData.MenuIdentifier;
                        menuEntity.PageUrl = menuData.PageUrl;
                        menuEntity.Icon = menuData.Icon;
                        menuEntity.LastModifiedBy = menuData.LastModifiedBy;
                        menuEntity.SubMenu = menuData.SubMenu;

                        menuEntity.SortOrder = item.Count + 1;
                        if (menuEntity.SubMenu != null && menuEntity.SubMenu.Count > 0)
                        {
                            for (int i = 0; i < menuEntity.SubMenu.Count; i++)
                            {
                                menuEntity.SubMenu[i].MenuMasterId = entity.SubMenu.Count > i ? entity.SubMenu[i].Id : null;
                                menuEntity.SubMenu[i].SortOrder = i + 1;
                            }
                        }
                        lstMenuData.Add(menuEntity);
                    }

                    UpdateMenuData(lstMenuData);
                }

            };
        }

        private void AddNewOrgnaizationMasterMenu(ManageMenuDataModel menuData, MenuMaster entity)
        {
            using (MongoRepository<MenuCustomization> menurepo = new MongoRepository<MenuCustomization>())
            {
                List<ManageMenuDataModel> lstMenuData = new List<ManageMenuDataModel>();
                var repodata = menurepo.ToList();
                var menuAccounts = repodata.GroupBy(info => info.AccountObjectID)
                 .Select(group => new
                 {
                     accountobjectid = group.Key,
                     Count = group.Count()
                 }).ToList();


                var selectedOrganization = menuData.OrganizationAccounts.Where(x => x.IsSelected);
                List<string> accountIds = menuAccounts.Select(x => x.accountobjectid).ToList();
                var accountsNotExistInCustomizeMenu = selectedOrganization.Where(x => !accountIds.Contains(x.AccountObjectID)).ToList();

                if (accountsNotExistInCustomizeMenu != null && accountsNotExistInCustomizeMenu.Count() > 0)
                {
                    lstMenuData = AddGolbalMenuToAccounts(accountsNotExistInCustomizeMenu);
                }


                lstMenuData = new List<ManageMenuDataModel>();
                accountIds = selectedOrganization.Select(x => x.AccountObjectID).ToList();
                repodata = menurepo.ToList();
                menuAccounts = repodata.GroupBy(info => info.AccountObjectID)
                .Select(group => new
                {
                    accountobjectid = group.Key,
                    Count = group.Count()
                }).Where(x => accountIds.Contains(x.accountobjectid)).ToList();

                foreach (var item in menuAccounts)
                {
                    ManageMenuDataModel menuEntity = new ManageMenuDataModel();

                    menuEntity.AccountObjectID = item.accountobjectid;
                    menuEntity.MenuMasterId = entity.Id;

                    menuEntity.Name = menuData.Name;
                    menuEntity.MenuIdentifier = menuData.MenuIdentifier;
                    menuEntity.PageUrl = menuData.PageUrl;
                    menuEntity.Icon = menuData.Icon;
                    menuEntity.LastModifiedBy = menuData.LastModifiedBy;
                    menuEntity.SubMenu = menuData.SubMenu;

                    menuEntity.SortOrder = item.Count + 1;
                    if (menuEntity.SubMenu != null && menuEntity.SubMenu.Count > 0)
                    {
                        for (int i = 0; i < menuEntity.SubMenu.Count; i++)
                        {
                            menuEntity.SubMenu[i].MenuMasterId = entity.SubMenu.Count > i ? entity.SubMenu[i].Id : null;
                            menuEntity.SubMenu[i].SortOrder = i + 1;
                        }
                    }
                    lstMenuData.Add(menuEntity);
                }

                UpdateMenuData(lstMenuData);
            }
        }

        private List<ManageMenuDataModel> AddGolbalMenuToAccounts(List<OrganizationAccount> accountsNotExistInCustomizeMenu)
        {
            List<ManageMenuDataModel> lstMenuData;
            using (MongoRepository<MenuMaster> repomenumaster = new MongoRepository<MenuMaster>())
            {
                lstMenuData = new List<ManageMenuDataModel>();
                var menuMasterData = repomenumaster.Where(x => x.MenuScope == MenuScopeKind.Global.ToString() || String.IsNullOrEmpty(x.MenuScope)).ToList();

                foreach (var item in accountsNotExistInCustomizeMenu)
                {
                    foreach (var mastermenuitem in menuMasterData)
                    {
                        ManageMenuDataModel menuEntity = new ManageMenuDataModel();
                        menuEntity.AccountObjectID = item.AccountObjectID;
                        menuEntity.MenuMasterId = mastermenuitem.Id;
                        menuEntity.SortOrder = mastermenuitem.SortOrder;
                        menuEntity.Name = mastermenuitem.Name;
                        menuEntity.MenuIdentifier = mastermenuitem.MenuIdentifier;
                        menuEntity.PageUrl = mastermenuitem.PageUrl;
                        menuEntity.Icon = mastermenuitem.Icon;
                        menuEntity.LastModifiedBy = mastermenuitem.LastModifiedBy;
                        menuEntity.SubMenu = CastToSubMenuData(mastermenuitem.SubMenu);
                        lstMenuData.Add(menuEntity);
                    }
                }
                UpdateMenuData(lstMenuData);
            }

            return lstMenuData;
        }

        public CreateNotificationReturn DeleteMenuMasterData(string menuMasterId, string subMenuMasterId)
        {
            try
            {
                using (MongoRepository<MenuMaster> repo = new MongoRepository<MenuMaster>())
                {
                    var data = repo.FirstOrDefault(x => x.Id == menuMasterId);
                    if (data != null)
                    {
                        if (String.IsNullOrEmpty(subMenuMasterId))
                        {
                            repo.Delete(data);

                            var lstMenuMaster = repo.Where(x => x.SortOrder > data.SortOrder).ToList();
                            foreach (var item in lstMenuMaster)
                            {
                                item.SortOrder = item.SortOrder - 1;
                            }
                            repo.Update(lstMenuMaster);

                            //---- Delete Account menu data----
                            DeleteAccountWiseMenu(menuMasterId);
                        }
                        else
                        {
                            if (data.SubMenu != null && data.SubMenu.Count > 0)
                            {
                                var subMenudata = data.SubMenu.FirstOrDefault(x => x.Id == subMenuMasterId);
                                data.SubMenu.Remove(subMenudata);
                                foreach (var item in data.SubMenu)
                                {
                                    if (item.SortOrder > subMenudata.SortOrder)
                                    {
                                        item.SortOrder = item.SortOrder - 1;
                                    }
                                }
                                repo.Update(data);

                                //---- Delete Account sub menu data----
                                DeleteAccountWiseSubMenu(menuMasterId, subMenuMasterId);
                            }
                        }
                    }

                    return new CreateNotificationReturn { Success = true, ObjectID = string.Empty };
                }
            }
            catch (Exception ex)
            {

                return new CreateNotificationReturn { Success = false, ObjectID = string.Empty };
            }
        }

        private static void DeleteAccountWiseMenu(string menuMasterId)
        {
            using (MongoRepository<MenuCustomization> menurepo = new MongoRepository<MenuCustomization>())
            {
                var repodata = menurepo.ToList();
                var menuAccounts = repodata.GroupBy(info => info.AccountObjectID).Select(group => new
                {
                    accountobjectid = group.Key,
                }).ToList();


                foreach (var item in menuAccounts)
                {
                    var accountMenus = menurepo.FirstOrDefault(x => x.AccountObjectID == item.accountobjectid && x.MenuMasterId == menuMasterId);
                    if (accountMenus != null)
                    {
                        menurepo.Delete(accountMenus);

                        var lstMenu = menurepo.Where(x => x.SortOrder > accountMenus.SortOrder && x.AccountObjectID == item.accountobjectid).ToList();
                        foreach (var itemmenu in lstMenu)
                        {
                            itemmenu.SortOrder = itemmenu.SortOrder - 1;
                        }
                        menurepo.Update(lstMenu);
                    }
                }
            }
        }

        private static void DeleteAccountWiseSubMenu(string menuMasterId, string subMenuMasterId)
        {
            using (MongoRepository<MenuCustomization> menurepo = new MongoRepository<MenuCustomization>())
            {
                var repodata = menurepo.ToList();
                var menuAccounts = repodata.GroupBy(info => info.AccountObjectID).Select(group => new
                {
                    accountobjectid = group.Key,
                }).ToList();


                foreach (var item in menuAccounts)
                {
                    var accountMenus = menurepo.FirstOrDefault(x => x.AccountObjectID == item.accountobjectid && x.MenuMasterId == menuMasterId);
                    if (accountMenus != null && accountMenus.SubMenu != null && accountMenus.SubMenu.Count > 0)
                    {
                        var subMenudata = accountMenus.SubMenu.FirstOrDefault(x => x.SubMenuMasterId == subMenuMasterId);
                        if (subMenudata != null)
                        {
                            accountMenus.SubMenu.Remove(subMenudata);
                            foreach (var itemmenu in accountMenus.SubMenu)
                            {
                                if (itemmenu.SortOrder > subMenudata.SortOrder)
                                {
                                    itemmenu.SortOrder = itemmenu.SortOrder - 1;
                                }
                            }
                            menurepo.Update(accountMenus);
                        }
                    }
                }
            }
        }

        private void SetMasterMenuToExistingAccounts(MenuMaster menuMasterData)
        {
            using (MongoRepository<MenuCustomization> menurepo = new MongoRepository<MenuCustomization>())
            {
                List<ManageMenuDataModel> lstMenuData = new List<ManageMenuDataModel>();
                var repodata = menurepo.ToList();
                var menuAccounts = repodata.GroupBy(info => info.AccountObjectID)
                 .Select(group => new
                 {
                     accountobjectid = group.Key,
                     Count = group.Count()
                 }).ToList();

                if (menuAccounts != null && menuAccounts.Count() > 0)
                {
                    List<MenuCustomization> lstMenu = new List<MenuCustomization>();
                    foreach (var item in menuAccounts)
                    {
                        var menudata = menurepo.FirstOrDefault(x => x.MenuMasterId == menuMasterData.Id && x.AccountObjectID == item.accountobjectid);

                        if (menudata != null)
                        {
                            List<SubMenuCustomization> lstSubMenu = new List<SubMenuCustomization>();
                            if (menuMasterData.SubMenu == null || menuMasterData.SubMenu.Count == 0)
                            {
                                menudata.SubMenu = null;
                            }
                            else if (menudata.SubMenu == null && menuMasterData.SubMenu != null && menuMasterData.SubMenu.Count > 0)
                            {
                                lstSubMenu = new List<SubMenuCustomization>();
                                SubMenuCustomization objSubMenu = new SubMenuCustomization();
                                foreach (var subitem in menuMasterData.SubMenu)
                                {
                                    objSubMenu = new SubMenuCustomization()
                                    {
                                        Id = Guid.NewGuid().ToString(),
                                        SubMenuMasterId = subitem.Id,
                                        Name = subitem.Name,
                                        //MenuIdentifier = subitem.MenuIdentifier,
                                        //PageUrl = subitem.PageUrl,
                                        Icon = subitem.Icon,
                                        SortOrder = subitem.SortOrder,
                                    };
                                    lstSubMenu.Add(objSubMenu);
                                }
                                menudata.SubMenu = lstSubMenu;
                            }
                            else
                            {
                                lstSubMenu = new List<SubMenuCustomization>();
                                SubMenuCustomization objSubMenu = new SubMenuCustomization();

                                foreach (var subitem in menuMasterData.SubMenu)
                                {
                                    var submenuexist = menudata.SubMenu.FirstOrDefault(x => x.SubMenuMasterId == subitem.Id);
                                    if (submenuexist == null)
                                    {
                                        objSubMenu = new SubMenuCustomization()
                                        {
                                            Id = Guid.NewGuid().ToString(),
                                            SubMenuMasterId = subitem.Id,
                                            Name = subitem.Name,
                                            //MenuIdentifier = subitem.MenuIdentifier,
                                            //PageUrl = subitem.PageUrl,
                                            Icon = subitem.Icon,
                                            SortOrder = subitem.SortOrder,
                                        };

                                        lstSubMenu.Add(objSubMenu);
                                    }
                                    else
                                    {
                                        lstSubMenu.Add(submenuexist);
                                    }
                                }

                                menudata.SubMenu = SetSortOrder(lstSubMenu);
                            }

                            lstMenu.Add(menudata);

                        }
                    }

                    menurepo.Update(lstMenu);
                }

            };
        }

        private void SetMasterMenuToGlobal(MenuMaster menuMasterData)
        {
            using (MongoRepository<MenuCustomization> menurepo = new MongoRepository<MenuCustomization>())
            {
                var repodata = menurepo.ToList();
                var menuAccounts = repodata.GroupBy(info => info.AccountObjectID)
                 .Select(group => new
                 {
                     accountobjectid = group.Key,
                     Count = group.Count()
                 }).ToList();

                if (menuAccounts != null && menuAccounts.Count() > 0)
                {
                    List<MenuCustomization> lstAddMenuData = new List<MenuCustomization>();
                    List<MenuCustomization> lstUpdateMenu = new List<MenuCustomization>();
                    foreach (var item in menuAccounts)
                    {
                        var menudata = menurepo.FirstOrDefault(x => x.MenuMasterId == menuMasterData.Id && x.AccountObjectID == item.accountobjectid);

                        if (menudata != null)
                        {
                            List<SubMenuCustomization> lstSubMenu = new List<SubMenuCustomization>();
                            if (menuMasterData.SubMenu == null || menuMasterData.SubMenu.Count == 0)
                            {
                                menudata.SubMenu = null;
                            }
                            else if (menudata.SubMenu == null && menuMasterData.SubMenu != null && menuMasterData.SubMenu.Count > 0)
                            {
                                lstSubMenu = new List<SubMenuCustomization>();
                                SubMenuCustomization objSubMenu = new SubMenuCustomization();
                                foreach (var subitem in menuMasterData.SubMenu)
                                {
                                    objSubMenu = new SubMenuCustomization()
                                    {
                                        Id = Guid.NewGuid().ToString(),
                                        SubMenuMasterId = subitem.Id,
                                        Name = subitem.Name,
                                        Icon = subitem.Icon,
                                        SortOrder = subitem.SortOrder,
                                    };
                                    lstSubMenu.Add(objSubMenu);
                                }
                                menudata.SubMenu = lstSubMenu;
                            }
                            else
                            {
                                lstSubMenu = new List<SubMenuCustomization>();
                                SubMenuCustomization objSubMenu = new SubMenuCustomization();

                                foreach (var subitem in menuMasterData.SubMenu)
                                {
                                    var submenuexist = menudata.SubMenu.FirstOrDefault(x => x.SubMenuMasterId == subitem.Id);
                                    if (submenuexist == null)
                                    {
                                        objSubMenu = new SubMenuCustomization()
                                        {
                                            Id = Guid.NewGuid().ToString(),
                                            SubMenuMasterId = subitem.Id,
                                            Name = subitem.Name,
                                            Icon = subitem.Icon,
                                            SortOrder = subitem.SortOrder,
                                        };

                                        lstSubMenu.Add(objSubMenu);
                                    }
                                    else
                                    {
                                        lstSubMenu.Add(submenuexist);
                                    }
                                }

                                menudata.SubMenu = SetSortOrder(lstSubMenu);
                            }

                            lstUpdateMenu.Add(menudata);

                        }
                        else
                        {
                            MenuCustomization menuEntity = new MenuCustomization();
                            menuEntity.AccountObjectID = item.accountobjectid;
                            menuEntity.MenuMasterId = menuMasterData.Id;

                            menuEntity.Name = menuMasterData.Name;
                            menuEntity.Icon = menuMasterData.Icon;
                            menuEntity.LastModifiedBy = menuMasterData.LastModifiedBy;
                            menuEntity.SubMenu = CastToSubMenuEntity(menuMasterData.SubMenu);

                            menuEntity.SortOrder = item.Count + 1;
                            if (menuEntity.SubMenu != null && menuEntity.SubMenu.Count > 0)
                            {
                                for (int i = 0; i < menuEntity.SubMenu.Count; i++)
                                {
                                    menuEntity.SubMenu[i].Id = menuMasterData.SubMenu.Count > i ? menuMasterData.SubMenu[i].Id : null;
                                    menuEntity.SubMenu[i].SortOrder = i + 1;
                                }
                            }
                            lstAddMenuData.Add(menuEntity);
                        }
                    }

                    menurepo.Update(lstUpdateMenu);
                    menurepo.Add(lstAddMenuData);
                }
            };
        }

        private void SetMasterMenuToOrganizationBased(ManageMenuDataModel menuData, MenuMaster menuMaster)
        {
            using (MongoRepository<MenuCustomization> menurepo = new MongoRepository<MenuCustomization>())
            {
                var repodata = menurepo.ToList();
                var menuAccounts = repodata.GroupBy(info => info.AccountObjectID)
                 .Select(group => new
                 {
                     accountobjectid = group.Key,
                     Count = group.Count()
                 }).ToList();

                var selectedOrganization = menuData.OrganizationAccounts.Where(x => x.IsSelected);
                List<string> accountIds = menuAccounts.Select(x => x.accountobjectid).ToList();
                var accountsNotExistInCustomizeMenu = selectedOrganization.Where(x => !accountIds.Contains(x.AccountObjectID)).ToList();
                bool isNewlyAddedOrganization = false;
                // Add menu for those accounts which are not exist customization table
                if (accountsNotExistInCustomizeMenu != null && accountsNotExistInCustomizeMenu.Count() > 0)
                {
                    List<ManageMenuDataModel> lstManageMenuData = new List<ManageMenuDataModel>();
                    lstManageMenuData = AddGolbalMenuToAccounts(accountsNotExistInCustomizeMenu);
                    if (lstManageMenuData.Count == 0)
                    {
                        isNewlyAddedOrganization = true;
                        // Add Menu to newly added organization
                        foreach (var item in accountsNotExistInCustomizeMenu)
                        {
                            AddNewOrgnaizationMasterMenu(item.AccountObjectID, menuMaster);
                        }
                    }
                }

                // Remove unselected accounts
                var accountsExistInCustomizeMenu = selectedOrganization.Where(x => accountIds.Contains(x.AccountObjectID)).ToList();
                accountIds = accountsExistInCustomizeMenu.Select(x => x.AccountObjectID).ToList();
                var accountsUnselectFromCustomizeMenu = menuData.OrganizationAccounts.Where(x => !accountIds.Contains(x.AccountObjectID)).ToList();
                if (isNewlyAddedOrganization)
                {
                    accountsUnselectFromCustomizeMenu = accountsUnselectFromCustomizeMenu.Except(accountsNotExistInCustomizeMenu).ToList();
                }

                if (accountsUnselectFromCustomizeMenu != null && accountsUnselectFromCustomizeMenu.Count() > 0)
                {
                    foreach (var item in accountsUnselectFromCustomizeMenu)
                    {
                        var removeOrgarnizationData = menurepo.FirstOrDefault(x => x.MenuMasterId == menuData.MenuMasterId && x.AccountObjectID == item.AccountObjectID);
                        if (removeOrgarnizationData != null)
                        {
                            menurepo.Delete(removeOrgarnizationData);
                        }
                    }
                }

                List<ManageMenuDataModel> lstMenuData = new List<ManageMenuDataModel>();
                accountIds = selectedOrganization.Select(x => x.AccountObjectID).ToList();
                repodata = menurepo.ToList(); // Get latest data
                menuAccounts = repodata.GroupBy(info => info.AccountObjectID)
                .Select(group => new
                {
                    accountobjectid = group.Key,
                    Count = group.Count()
                }).Where(x => accountIds.Contains(x.accountobjectid)).ToList();

                foreach (var item in menuAccounts)
                {
                    ManageMenuDataModel menuEntity = new ManageMenuDataModel();

                    menuEntity.AccountObjectID = item.accountobjectid;
                    var customizeMenuData = repodata.FirstOrDefault(x => x.MenuMasterId == menuData.MenuMasterId && x.AccountObjectID == item.accountobjectid);
                    if (customizeMenuData != null)
                    {
                        menuEntity.MenuID = customizeMenuData.Id;
                    }
                    menuEntity.MenuMasterId = menuData.MenuMasterId;
                    menuEntity.Name = menuData.Name;
                    menuEntity.MenuIdentifier = menuData.MenuIdentifier;
                    menuEntity.PageUrl = menuData.PageUrl;
                    menuEntity.Icon = menuData.Icon;
                    menuEntity.LastModifiedBy = menuData.LastModifiedBy;
                    menuEntity.SubMenu = menuData.SubMenu;

                    menuEntity.SortOrder = item.Count + 1;
                    if (menuEntity.SubMenu != null && menuEntity.SubMenu.Count > 0)
                    {
                        for (int i = 0; i < menuEntity.SubMenu.Count; i++)
                        {
                            menuEntity.SubMenu[i].MenuMasterId = menuData.SubMenu.Count > i ? menuData.SubMenu[i].MenuMasterId : null;
                            if (String.IsNullOrEmpty(menuEntity.SubMenu[i].MenuMasterId))
                            {
                                menuEntity.SubMenu[i].MenuMasterId = menuMaster.SubMenu.Count > i ? menuMaster.SubMenu[i].Id : null;
                            }
                            menuEntity.SubMenu[i].SortOrder = i + 1;
                        }
                    }
                    lstMenuData.Add(menuEntity);
                }

                UpdateMenuData(lstMenuData);

            };
        }

        private void AddNewOrgnaizationMasterMenu(string accountObjectID, MenuMaster mastermenuitem)
        {
            List<ManageMenuDataModel> lstMenuData = new List<ManageMenuDataModel>();
            ManageMenuDataModel menuEntity = new ManageMenuDataModel();
            menuEntity.AccountObjectID = accountObjectID;
            menuEntity.MenuMasterId = mastermenuitem.Id;
            menuEntity.SortOrder = mastermenuitem.SortOrder;
            menuEntity.Name = mastermenuitem.Name;
            menuEntity.MenuIdentifier = mastermenuitem.MenuIdentifier;
            menuEntity.PageUrl = mastermenuitem.PageUrl;
            menuEntity.Icon = mastermenuitem.Icon;
            menuEntity.LastModifiedBy = mastermenuitem.LastModifiedBy;
            menuEntity.SubMenu = CastToSubMenuData(mastermenuitem.SubMenu);
            lstMenuData.Add(menuEntity);

            UpdateMenuData(lstMenuData);
        }
        #endregion

        #region Helper Methods
        private List<ManageMenuDataModel> CastToSubMenuData(List<SubMenuCustomization> subMenu)
        {
            if (subMenu != null && subMenu.Count > 0)
            {
                List<ManageMenuDataModel> lstSubMenuData = new List<ManageMenuDataModel>();
                foreach (var item in subMenu)
                {
                    lstSubMenuData.Add(new ManageMenuDataModel
                    {
                        MenuID = item.Id,
                        MenuMasterId = item.SubMenuMasterId,
                        Name = item.Name,
                        Icon = item.Icon,
                        SortOrder = item.SortOrder,
                        SubMenu = CastToSubMenuData(item.SubMenus),
                    });
                }
                return lstSubMenuData;
            }
            else
            {
                return null;
            }
        }

        private List<ManageMenuDataModel> CastToSubMenuData(List<SubMenuMaster> subMenu)
        {
            if (subMenu != null && subMenu.Count > 0)
            {
                subMenu = subMenu.Where(x => x.Status).ToList();
                List<ManageMenuDataModel> lstSubMenuData = new List<ManageMenuDataModel>();
                foreach (var item in subMenu)
                {
                    lstSubMenuData.Add(new ManageMenuDataModel
                    {
                        MenuID = item.Id,
                        MenuMasterId = item.Id,
                        Name = item.Name,
                        MasterMenuName = item.Name,
                        MenuIdentifier = item.MenuIdentifier,
                        PageUrl = item.PageUrl,
                        Icon = item.Icon,
                        SortOrder = item.SortOrder,
                        SubMenu = CastToSubMenuData(item.SubMenus),
                    });
                }
                return lstSubMenuData;
            }
            else
            {
                return null;
            }
        }

        private List<SubMenuCustomization> CastToSubMenuEntity(List<ManageMenuDataModel> menuData)
        {
            if (menuData != null && menuData.Count > 0)
            {
                List<SubMenuCustomization> lstMenu = new List<SubMenuCustomization>();
                foreach (var item in menuData)
                {
                    lstMenu.Add(new SubMenuCustomization
                    {
                        Id = Guid.NewGuid().ToString(),
                        SubMenuMasterId = item.MenuMasterId,
                        Name = item.Name,
                        Icon = item.Icon,
                        SortOrder = item.SortOrder,
                        SubMenus = CastToSubMenuEntity(item.SubMenu),
                    });
                }
                return lstMenu;
            }
            else
            {
                return null;
            }
        }

        private List<SubMenuMaster> CastToSubMenuMasterEntity(List<ManageMenuDataModel> subMenu)
        {
            if (subMenu != null && subMenu.Count > 0)
            {
                List<SubMenuMaster> lstSubMenuData = new List<SubMenuMaster>();
                foreach (var item in subMenu)
                {
                    lstSubMenuData.Add(new SubMenuMaster
                    {
                        Id = string.IsNullOrEmpty(item.MenuID) ? Guid.NewGuid().ToString() : item.MenuID,
                        Name = item.Name,
                        MenuIdentifier = item.MenuIdentifier,
                        PageUrl = item.PageUrl,
                        Icon = item.Icon,
                        SortOrder = item.SortOrder,
                        Status = item.Status,
                        SubMenus = CastToSubMenuMasterEntity(item.SubMenu),
                    });
                }
                return lstSubMenuData;
            }
            else
            {
                return null;
            }
        }

        private List<SubMenuCustomization> CastToSubMenuEntity(List<SubMenuMaster> menuData)
        {
            if (menuData != null && menuData.Count > 0)
            {
                List<SubMenuCustomization> lstMenu = new List<SubMenuCustomization>();
                foreach (var item in menuData)
                {
                    lstMenu.Add(new SubMenuCustomization
                    {
                        Id = Guid.NewGuid().ToString(),
                        SubMenuMasterId = item.Id,
                        Name = item.Name,
                        Icon = item.Icon,
                        SortOrder = item.SortOrder,
                        SubMenus = CastToSubMenuEntity(item.SubMenus),
                    });
                }
                return lstMenu;
            }
            else
            {
                return null;
            }
        }

        private List<OrganizationAccount> GetOrganization(string menuMasterId)
        {
            List<OrganizationAccount> lstAccountOrganization = new List<OrganizationAccount>();
            using (MongoRepository<MenuCustomization> repo = new MongoRepository<MenuCustomization>())
            {
                var data = repo.Where(x => x.MenuMasterId == menuMasterId).ToList();
                foreach (var item in data)
                {
                    lstAccountOrganization.Add(new OrganizationAccount
                    {
                        AccountObjectID = item.AccountObjectID,
                        IsSelected = true,
                    });
                }
            }
            return lstAccountOrganization;
        }

        private List<SubMenuCustomization> SetSortOrder(List<SubMenuCustomization> SubMenu)
        {
            foreach (var item in SubMenu)
            {
                item.SortOrder = SubMenu.IndexOf(item) + 1;
            }
            return SubMenu;
        }

        #endregion

    }
}
