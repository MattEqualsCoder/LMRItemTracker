using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;
using System.Collections.Generic;
using LMRItemTracker.VoiceTracker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using LMRItemTracker.Configs;

namespace LMRItemTracker
{
    public partial class LaMulanaItemTrackerForm : Form
    {
        public static bool DialogOpen { get; set; }
        private BackgroundWorker flagListener;
        private ILogger<LaMulanaItemTrackerForm> _logger;
        private readonly TrackerService _trackerService;
        private readonly VoiceRecognitionService _voiceRecognitionService;
        private readonly IServiceProvider _services;
        private readonly ChatModule _chatModule;
        private readonly TrackerConfig _config;
        private readonly Dictionary<string, Label> _regionLabels = new();

        public LaMulanaItemTrackerForm(ILogger<LaMulanaItemTrackerForm> logger, TrackerService trackerService, HintService hintService, VoiceRecognitionService voiceService, IServiceProvider services, ChatModule chatModule, ConfigService configService)
        {
            flagListener = new();
            _logger = logger;
            _trackerService = trackerService;
            InitializeComponent();
            gameStarted = false;
            _voiceRecognitionService = voiceService;
            _services = services;
            _chatModule = chatModule;
            _trackerService.TrackerForm = this;
            _config = configService.Config;
        }

        private void ScaleImages(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                if (c is PictureBox p)
                {
                    p.BackgroundImageLayout = ImageLayout.Zoom;
                    p.SizeMode = PictureBoxSizeMode.Zoom;
                }
                ScaleImages(c);
            }
        }

        private void InitializeBackgroundWorker()
        {
            flagListener = new BackgroundWorker();
            flagListener.DoWork += flagListener_DoWork;
        }

        private void InitializePossibleItems()
        {
            allItems = new List<String>(92);
            allItems.Add("Anchor");
            allItems.Add("Ankh Jewels");
            allItems.Add("Axe");
            allItems.Add("Birth Seal");
            allItems.Add("Bomb");
            allItems.Add("Book of the Dead");
            allItems.Add("Bracelet");
            allItems.Add("Bronze Mirror");
            allItems.Add("beolamu.exe");
            allItems.Add("bounce.exe");
            allItems.Add("bunemon.exe");
            allItems.Add("bunplus.com");
            allItems.Add("Caltrops");
            allItems.Add("capstar.exe");
            allItems.Add("Chakram");
            allItems.Add("Cog of the Soul");
            allItems.Add("Crucifix");
            allItems.Add("Crystal Skull");
            allItems.Add("Death Seal");
            allItems.Add("deathv.exe");
            allItems.Add("Diary");
            allItems.Add("Dimensional Key");
            allItems.Add("Djed Pillar");
            allItems.Add("Dragon Bone");
            allItems.Add("Earth Spear");
            allItems.Add("emusic.exe");
            allItems.Add("Eye of Truth");
            allItems.Add("Fairy Clothes");
            allItems.Add("Feather");
            allItems.Add("Flare Gun");
            allItems.Add("Fruit of Eden");
            allItems.Add("Gauntlet");
            allItems.Add("Glove");
            allItems.Add("reader.exe");
            allItems.Add("Grapple Claw");
            allItems.Add("guild.exe");
            allItems.Add("Hand Scanner");
            allItems.Add("Heatproof Case");
            allItems.Add("Helmet");
            allItems.Add("Hermes' Boots");
            allItems.Add("Holy Grail");
            allItems.Add("Ice Cape");
            allItems.Add("Isis' Pendant");
            allItems.Add("Katana");
            allItems.Add("Key Fairy Combo");
            allItems.Add("Key of Eternity");
            allItems.Add("Key Sword");
            allItems.Add("Knife");
            allItems.Add("Lamp of Time");
            allItems.Add("lamulana.exe");
            allItems.Add("Life Seal");
            allItems.Add("Magatama Jewel");
            allItems.Add("Mantra/Djed Pillar");
            allItems.Add("mantra.exe");
            allItems.Add("Maps");
            allItems.Add("mekuri.exe");
            allItems.Add("Mini Doll");
            allItems.Add("miracle.exe");
            allItems.Add("mirai.exe");
            allItems.Add("Mobile Super X2");
            allItems.Add("move.exe");
            allItems.Add("Mulana Talisman");
            allItems.Add("Origin Seal");
            allItems.Add("Pepper");
            allItems.Add("Perfume");
            allItems.Add("Philosopher's Ocarina");
            allItems.Add("Pistol");
            allItems.Add("Plane Model");
            allItems.Add("Pochette Key");
            allItems.Add("Provocative Bathing Suit");
            allItems.Add("randc.exe");
            allItems.Add("Ring");
            allItems.Add("Rolling Shuriken");
            allItems.Add("Scalesphere");
            allItems.Add("Scriptures");
            allItems.Add("Serpent Staff");
            allItems.Add("Shield");
            allItems.Add("Shell Horn");
            allItems.Add("Shrine Wall Removal");
            allItems.Add("Shuriken");
            allItems.Add("Spaulder");
            allItems.Add("Talisman");
            allItems.Add("torude.exe");
            allItems.Add("Treasures");
            allItems.Add("Twin Statue");
            allItems.Add("Vessel/Medicine");
            allItems.Add("Waterproof Case");
            allItems.Add("Whip");
            allItems.Add("Woman Statue");
            allItems.Add("xmailer.exe");
            allItems.Add("yagomap.exe");
            allItems.Add("yagostr.exe");
        }

        private void InitializeFormPanels()
        {
            foreach (String item in allItems)
            {
                Control? control = GetControl(item);
                if (control != null)
                {
                    if (control is TrackerBox)
                    {
                        ((TrackerBox)control).ToggleState(false);
                    }
                    else if (control is ItemTextPanel)
                    {
                        ((ItemTextPanel)control).ToggleState(false);
                    }
                    else if (control is PistolPanel)
                    {
                        ((PistolPanel)control).ToggleState(false);
                    }
                    else if (control is KeySwordTrackerBox)
                    {
                        ((KeySwordTrackerBox)control).ToggleState(false, true);
                    }
                    else if (control is KeyFairyTrackerBox)
                    {
                        ((KeyFairyTrackerBox)control).ToggleState(false, true);
                    }
                    else if (control is MultiStateTrackerBox)
                    {
                        ((MultiStateTrackerBox)control).ToggleState(false, 0);
                    }
                    control.Parent = null;
                }
            }
            whip.ToggleState(true, 2);
            shrinePanel.ToggleState(true);
            mapsPanel.ToggleState(true);

            List<String> itemsInPanel = new List<String>(Properties.Settings.Default.Panel1Contents.Split(','));
            for(int index = 0; index < itemsInPanel.Count; index++)
            {
                String item = itemsInPanel[index];
                Control? control = GetControl(item);
                if(control != null)
                {
                    flowLayoutPanel1.Controls.Add(control);
                    control.Margin = new Padding(0);
                    control.TabIndex = index;
                }
            }
            if(flowLayoutPanel1.Controls.Count < 1)
            {
                flowLayoutPanel1.Visible = false;
            }

            itemsInPanel = new List<String>(Properties.Settings.Default.Panel2Contents.Split(','));
            for (int index = 0; index < itemsInPanel.Count; index++)
            {
                String item = itemsInPanel[index];
                Control? control = GetControl(item);
                if (control != null)
                {
                    flowLayoutPanel2.Controls.Add(control);
                    control.Margin = new Padding(0);
                    control.TabIndex = index;
                }
            }
            if (flowLayoutPanel2.Controls.Count < 1)
            {
                flowLayoutPanel2.Visible = false;
            }

            itemsInPanel = new List<String>(Properties.Settings.Default.Panel3Contents.Split(','));
            for (int index = 0; index < itemsInPanel.Count; index++)
            {
                String item = itemsInPanel[index];
                Control? control = GetControl(item);
                if (control != null)
                {
                    flowLayoutPanel3.Controls.Add(control);
                    control.Margin = new Padding(0);
                    control.TabIndex = index;
                }
            }
            if (flowLayoutPanel3.Controls.Count < 1)
            {
                flowLayoutPanel3.Visible = false;
            }

            itemsInPanel = new List<String>(Properties.Settings.Default.Panel4Contents.Split(','));
            for (int index = 0; index < itemsInPanel.Count; index++)
            {
                String item = itemsInPanel[index];
                Control? control = GetControl(item);
                if (control != null)
                {
                    flowLayoutPanel4.Controls.Add(control);
                    control.Margin = new Padding(0);
                    control.TabIndex = index;
                }
            }
            if (flowLayoutPanel4.Controls.Count < 1)
            {
                flowLayoutPanel4.Visible = false;
            }

            itemsInPanel = new List<String>(Properties.Settings.Default.Panel5Contents.Split(','));
            for (int index = 0; index < itemsInPanel.Count; index++)
            {
                String item = itemsInPanel[index];
                Control? control = GetControl(item);
                if (control != null)
                {
                    flowLayoutPanel5.Controls.Add(control);
                    control.Margin = new Padding(0);
                    control.TabIndex = index;
                }
            }
            if (flowLayoutPanel5.Controls.Count < 1)
            {
                flowLayoutPanel5.Visible = false;
            }

            itemsInPanel = new List<String>(Properties.Settings.Default.Panel6Contents.Split(','));
            for (int index = 0; index < itemsInPanel.Count; index++)
            {
                String item = itemsInPanel[index];
                Control? control = GetControl(item);
                if (control != null)
                {
                    flowLayoutPanel6.Controls.Add(control);
                    control.Margin = new Padding(0);
                    control.TabIndex = index;
                }
            }
            if (flowLayoutPanel6.Controls.Count < 1)
            {
                flowLayoutPanel6.Visible = false;
            }
        }

        private void InitializeMenuOptions()
        {
            foreach (String item in allItems)
            {
                addItemPanel1ToolStripMenuItem.DropDownItems.Add(CreateMenuItem(item, AddItemToPanel1));
                addItemPanel2ToolStripMenuItem.DropDownItems.Add(CreateMenuItem(item, AddItemToPanel2));
                addItemPanel3ToolStripMenuItem.DropDownItems.Add(CreateMenuItem(item, AddItemToPanel3));
                addItemPanel4ToolStripMenuItem.DropDownItems.Add(CreateMenuItem(item, AddItemToPanel4));
                addItemPanel5ToolStripMenuItem.DropDownItems.Add(CreateMenuItem(item, AddItemToPanel5));
                addItemPanel6ToolStripMenuItem.DropDownItems.Add(CreateMenuItem(item, AddItemToPanel6));
                removeItemToolStripMenuItem.DropDownItems.Add(CreateMenuItem(item, RemoveItemFromPanel));
            }
        }

        private void flagListener_DoWork(object sender, DoWorkEventArgs e)
        {
            // Get the BackgroundWorker that raised this event.
            BackgroundWorker? worker = sender as BackgroundWorker;

            // Assign the result of the computation
            // to the Result property of the DoWorkEventArgs
            // object. This is will be available to the 
            // RunWorkerCompleted eventhandler.
            try
            {
                e.Result = Program.DoStuff(this, _xmlStream);
            }
            catch (Exception ex)
            {
                ShowMessage("Unexpected error: " + ex.Message);
            }
        }

        public void ShowMessage(string message)
        {
            MessageBox.Show(message);
        }

        private String? getItemName(String flagName)
        {
            if ("w-scanner".Equals(flagName))
            {
                return "Hand Scanner";
            }
            else if ("w-grail".Equals(flagName))
            {
                return "Holy Grail";
            }
            else if ("w-doll".Equals(flagName))
            {
                return "Mini Doll";
            }
            else if ("w-magatama".Equals(flagName))
            {
                return "Magatama Jewel";
            }
            else if ("w-pepper".Equals(flagName))
            {
                return "Pepper";
            }
            else if ("w-woman".Equals(flagName))
            {
                return "Woman Statue";
            }
            else if ("w-serpent".Equals(flagName))
            {
                return "Serpent Staff";
            }
            else if ("w-glove".Equals(flagName))
            {
                return "Glove";
            }
            else if ("w-crucifix".Equals(flagName))
            {
                return "Crucifix";
            }
            else if ("w-eye-truth".Equals(flagName))
            {
                return "Eye of Truth";
            }
            else if ("w-scale".Equals(flagName))
            {
                return "Scalesphere";
            }
            else if ("w-gauntlet".Equals(flagName))
            {
                return "Gauntlet";
            }
            else if ("w-anchor".Equals(flagName))
            {
                return "Anchor";
            }
            else if ("w-book".Equals(flagName))
            {
                return "Book of the Dead";
            }
            else if ("w-clothes".Equals(flagName))
            {
                return "Fairy Clothes";
            }
            else if ("w-scriptures".Equals(flagName))
            {
                return "Scriptures";
            }
            else if ("w-bracelet".Equals(flagName))
            {
                return "Bracelet";
            }
            else if ("w-perfume".Equals(flagName))
            {
                return "Perfume";
            }
            else if ("w-spaulder".Equals(flagName))
            {
                return "Spaulder";
            }
            else if ("w-icecape".Equals(flagName))
            {
                return "Ice Cape";
            }
            else if ("w-talisman".Equals(flagName))
            {
                return "Talisman";
            }
            else if ("w-diary".Equals(flagName))
            {
                return "Diary";
            }
            else if ("w-mulanatalisman".Equals(flagName))
            {
                return "Mulana Talisman";
            }
            else if ("w-dimension-key".Equals(flagName))
            {
                return "Dimensional Key";
            }
            else if ("w-cog".Equals(flagName))
            {
                return "Cog of the Soul";
            }
            else if ("w-cskull".Equals(flagName))
            {
                return "Crystal Skull";
            }
            else if ("w-endless-key".Equals(flagName))
            {
                return "Key of Eternity";
            }
            else if ("w-isispendant".Equals(flagName))
            {
                return "Isis' Pendant";
            }
            else if ("w-helmet".Equals(flagName))
            {
                return "Helmet";
            }
            else if ("w-grapple".Equals(flagName))
            {
                return "Grapple Claw";
            }
            else if ("w-mirror".Equals(flagName))
            {
                return "Bronze Mirror";
            }
            else if ("w-ring".Equals(flagName))
            {
                return "Ring";
            }
            else if ("w-plane".Equals(flagName))
            {
                return "Plane Model";
            }
            else if ("w-ocarina".Equals(flagName))
            {
                return "Philosopher's Ocarina";
            }
            else if ("w-feather".Equals(flagName))
            {
                return "Feather";
            }
            else if ("w-hermes".Equals(flagName))
            {
                return "Hermes' Boots";
            }
            else if ("w-fruit".Equals(flagName))
            {
                return "Fruit of Eden";
            }
            else if ("w-twin-statue".Equals(flagName))
            {
                return "Twin Statue";
            }
            else if ("w-treasures".Equals(flagName))
            {
                return "Treasures";
            }
            else if ("w-pochettekey".Equals(flagName))
            {
                return "Pochette Key";
            }
            else if ("w-msx2".Equals(flagName))
            {
                return "Mobile Super X2";
            }
            else if ("w-shell-horn".Equals(flagName))
            {
                return "Shell Horn";
            }
            else if ("w-heat-case".Equals(flagName))
            {
                return "Heatproof Case";
            }
            else if ("w-water-case".Equals(flagName))
            {
                return "Waterproof Case";
            }
            else if ("w-djed".Equals(flagName))
            {
                return "Djed Pillar";
            }
            else if ("w-dragonbone".Equals(flagName))
            {
                return "Dragon Bone";
            }
            else if ("w-seal1".Equals(flagName))
            {
                return "Origin Seal";
            }
            else if ("w-seal2".Equals(flagName))
            {
                return "Birth Seal";
            }
            else if ("w-seal3".Equals(flagName))
            {
                return "Life Seal";
            }
            else if ("w-seal4".Equals(flagName))
            {
                return "Death Seal";
            }
            else if ("w-soft-beolamu".Equals(flagName))
            {
                return "beolamu.exe";
            }
            else if ("w-soft-bounce".Equals(flagName))
            {
                return "bounce.exe";
            }
            else if ("w-soft-bunemon".Equals(flagName))
            {
                return "bunemon.exe";
            }
            else if ("w-soft-bunplus".Equals(flagName))
            {
                return "bunplus.com";
            }
            else if ("w-soft-capstar".Equals(flagName))
            {
                return "capstar.exe";
            }
            else if ("w-soft-deathv".Equals(flagName))
            {
                return "deathv.exe";
            }
            else if ("w-soft-emusic".Equals(flagName))
            {
                return "emusic.exe";
            }
            else if ("w-soft-guild".Equals(flagName))
            {
                return "guild.exe";
            }
            else if ("w-soft-lamulana".Equals(flagName))
            {
                return "lamulana.exe";
            }
            else if ("w-soft-mantra".Equals(flagName))
            {
                return "mantra.exe";
            }
            else if ("w-soft-mekuri".Equals(flagName))
            {
                return "mekuri.exe";
            }
            else if ("w-soft-mirai".Equals(flagName))
            {
                return "mirai.exe";
            }
            else if ("w-soft-miracle".Equals(flagName))
            {
                return "miracle.exe";
            }
            else if ("w-soft-move".Equals(flagName))
            {
                return "move.exe";
            }
            else if ("w-soft-randc".Equals(flagName))
            {
                return "randc.exe";
            }
            else if ("w-soft-reader".Equals(flagName))
            {
                return "reader.exe";
            }
            else if ("w-soft-torude".Equals(flagName))
            {
                return "torude.exe";
            }
            else if ("w-soft-xmailer".Equals(flagName))
            {
                return "xmailer.exe";
            }
            else if ("w-soft-yagomap".Equals(flagName))
            {
                return "yagomap.exe";
            }
            else if ("w-soft-yagostr".Equals(flagName))
            {
                return "yagostr.exe";
            }
            else if ("w-main-axe".Equals(flagName))
            {
                return "Axe";
            }
            else if ("w-main-knife".Equals(flagName))
            {
                return "Knife";
            }
            else if ("w-main-katana".Equals(flagName))
            {
                return "Katana";
            }
            else if ("w-sub-shuriken".Equals(flagName))
            {
                return "Shuriken";
            }
            else if ("w-sub-rshuriken".Equals(flagName))
            {
                return "Rolling Shuriken";
            }
            else if ("w-sub-caltrops".Equals(flagName))
            {
                return "Caltrops";
            }
            else if ("w-sub-spear".Equals(flagName))
            {
                return "Earth Spear";
            }
            else if ("w-sub-flare".Equals(flagName))
            {
                return "Flare Gun";
            }
            else if ("w-sub-bomb".Equals(flagName))
            {
                return "Bomb";
            }
            else if ("w-sub-chakram".Equals(flagName))
            {
                return "Chakram";
            }
            else if ("w-sub-pistol".Equals(flagName))
            {
                return "Pistol";
            }
            else if ("ankh-jewels".Equals(flagName))
            {
                return "Ankh Jewels";
            }
            else if ("shield-buckler".Equals(flagName))
            {
                return "Buckler";
            }
            else if ("whip".Equals(flagName))
            {
                return "Whip";
            }
            else if("w-forbidden".Equals(flagName))
            {
                return "Provocative Bathing Suit";
            }
            return null;
        }

        public void toggleItem(string flagName, bool isAdd)
        {
            _logger.LogInformation("toggleItem: {Name} | {Value}", flagName, isAdd);
            if ("w-scanner".Equals(flagName) || "w-doll".Equals(flagName) || "w-magatama".Equals(flagName)
                || "w-cog".Equals(flagName) || "w-pochettekey".Equals(flagName) || "w-cskull".Equals(flagName)
                || "w-pepper".Equals(flagName) || "w-endless-key".Equals(flagName)
                || "w-serpent".Equals(flagName) || "w-talisman".Equals(flagName) || "w-diary".Equals(flagName)
                || "w-mulanatalisman".Equals(flagName))
            {
                // Usable items with no special image handling
                SetImage(flagName, isAdd);
                _trackerService.SetItemState(flagName, isAdd);
            }
            else if ("w-msx2".Equals(flagName) || "w-heat-case".Equals(flagName) || "w-water-case".Equals(flagName)
                || "w-shell-horn".Equals(flagName) || "w-glove".Equals(flagName)
                || "w-isispendant".Equals(flagName) || "w-crucifix".Equals(flagName) || "w-helmet".Equals(flagName)
                || "w-grapple".Equals(flagName) || "w-mirror".Equals(flagName) || "w-eye-truth".Equals(flagName)
                || "w-ring".Equals(flagName) || "w-scale".Equals(flagName) || "w-gauntlet".Equals(flagName)
                || "w-anchor".Equals(flagName) || "w-treasures".Equals(flagName) || "w-plane".Equals(flagName)
                || "w-ocarina".Equals(flagName) || "w-feather".Equals(flagName) || "w-book".Equals(flagName)
                || "w-clothes".Equals(flagName) || "w-scriptures".Equals(flagName) || "w-hermes".Equals(flagName)
                || "w-fruit".Equals(flagName) || "w-twin-statue".Equals(flagName) || "w-bracelet".Equals(flagName)
                || "w-perfume".Equals(flagName) || "w-spaulder".Equals(flagName) || "w-dimension-key".Equals(flagName)
                || "w-icecape".Equals(flagName) || "w-forbidden".Equals(flagName))
            {
                // Non-usable items with no special image handling
                SetImage(flagName, isAdd);
                _trackerService.SetItemState(flagName, isAdd);
            }
            else if("w-grail".Equals(flagName))
            {
                holyGrail.ToggleState(isAdd, 2);
                _trackerService.SetItemState(flagName, isAdd);
            }
            else if ("w-woman".Equals(flagName))
            {
                womanStatue.ToggleState(isAdd, 1);
                _trackerService.SetItemState(flagName, isAdd);
            }
            else if ("w-main-chain".Equals(flagName))
            {
                whip.ToggleState(isAdd, 1);
                _trackerService.SetItemState(flagName, isAdd);
            }
            else if ("w-main-flail".Equals(flagName))
            {
                whip.ToggleState(isAdd, 0);
                _trackerService.SetItemState(flagName, isAdd);
            }
            else if ("w-main-keysword".Equals(flagName))
            {
                keySword.ToggleState(isAdd, true);
                _trackerService.SetItemState(flagName, isAdd);
            }
            else if (flagName.StartsWith("w-main-"))
            {
                SetImage(flagName, isAdd);
                _trackerService.SetItemState(flagName, isAdd);
            }
            else if (flagName.StartsWith("w-sub-"))
            {
                toggleSubWeapon(flagName, isAdd);
                _trackerService.SetItemState(flagName, isAdd);
            }
            else if ("w-soft-mantra".Equals(flagName))
            {
                SetImage(flagName, isAdd);
                mantra.ToggleState(isAdd);
                _trackerService.SetItemState(flagName, isAdd);
            }
            else if (flagName.StartsWith("w-soft-"))
            {
                SetImage(flagName, isAdd);
                if ("w-soft-mekuri".Equals(flagName))
                {
                    keyFairy.ToggleState(isAdd, false);
                }
                else if ("w-soft-miracle".Equals(flagName))
                {
                    keyFairy.ToggleState(isAdd, true);
                }
                else if ("w-soft-yagomap".Equals(flagName))
                {
                    shrinePanel.UpdateCount(isAdd);
                }
                else if ("w-soft-yagostr".Equals(flagName))
                {
                    shrinePanel.UpdateCount(isAdd);
                }
                _trackerService.SetItemState(flagName, isAdd);
            }
            else if ("w-seal1".Equals(flagName) || "w-seal2".Equals(flagName) || "w-seal3".Equals(flagName) || "w-seal4".Equals(flagName))
            {
                SetImage(flagName, isAdd);
                _trackerService.SetItemState(flagName, isAdd);
            }
            else if ("w-maternity".Equals(flagName))
            {
                womanStatue.ToggleState(isAdd, 0);
                _trackerService.SetItemState(flagName, isAdd);
            }
            else if ("w-vessel".Equals(flagName))
            {
                vessel.ToggleState(isAdd, 3);
                _trackerService.SetItemState(flagName, isAdd);
            }
            else if ("w-lamp".Equals(flagName))
            {
                lampOfTime.ToggleState(isAdd, 1);
                _trackerService.SetItemState(flagName, isAdd);
            }
            else if ("w-djed".Equals(flagName))
            {
                SetImage(flagName, isAdd);
                mantra.ToggleForeState(isAdd);
                _trackerService.SetItemState(flagName, isAdd);
            }
            else if ("w-dragonbone".Equals(flagName))
            {
                shrinePanel.UpdateCount(isAdd);
                SetImage(flagName, isAdd);
                _trackerService.SetItemState(flagName, isAdd);
            }
            else if ("w-medicine-yellow".Equals(flagName))
            {
                vessel.ToggleState(isAdd, 2);
            }
            else if ("w-medicine-red".Equals(flagName))
            {
                vessel.ToggleState(isAdd, 1);
            }
            else if ("w-medicine-green".Equals(flagName))
            {
                vessel.ToggleState(isAdd, 0);
            }
            else if (flagName.StartsWith("w-orb-"))
            {
                _trackerService.SetItemState(flagName, isAdd);
            }
        }

        public void Log(string message)
        {
            _logger.LogInformation(message);
        }

        private void UpdatePanelControls(Control panel, Control foundControl, Control blankControl, bool isAdd)
        {
            if (isAdd)
            {
                panel.Invoke(new Action(() =>
                {
                    panel.Controls.Add(foundControl);
                    panel.Controls.Remove(blankControl);
                }));
            }
            else
            {
                panel.Invoke(new Action(() =>
                {
                    panel.Controls.Add(blankControl);
                    panel.Controls.Remove(foundControl);
                }));
            }
        }

        private void toggleSubWeapon(string flagName, bool isAdd)
        {
            _logger.LogInformation("toggleSubWeapon: {Name} | {Value}", flagName, isAdd);
            SetImage(flagName, isAdd);
            if ("w-sub-shuriken".Equals(flagName))
            {
                shurikenPanel.ToggleState(isAdd);
            }
            else if ("w-sub-rshuriken".Equals(flagName))
            {
                rollingShurikenPanel.ToggleState(isAdd);
            }
            else if ("w-sub-caltrops".Equals(flagName))
            {
                caltropsPanel.ToggleState(isAdd);
            }
            else if ("w-sub-spear".Equals(flagName))
            {
                earthSpearPanel.ToggleState(isAdd);
            }
            else if ("w-sub-flare".Equals(flagName))
            {
                flareGunPanel.ToggleState(isAdd);
            }
            else if ("w-sub-bomb".Equals(flagName))
            {
                bombPanel.ToggleState(isAdd);
            }
            else if ("w-sub-chakram".Equals(flagName))
            {
                chakramPanel.ToggleState(isAdd);
            }
            else if ("w-sub-pistol".Equals(flagName))
            {
                pistolPanel.ToggleState(isAdd);
            }
        }

        public void ToggleGrail(string flagName, bool isAdd)
        {
            _logger.LogInformation("toggleGrail: {Name} | {Value}", flagName, isAdd);
            if ("invtr-grailfull".Equals(flagName))
            {
                holyGrail.ToggleState(isAdd, 1);
            }
            else if ("invtr-grailbr".Equals(flagName))
            {
                holyGrail.ToggleState(isAdd, 0);
            }
            _trackerService.SetItemState(flagName, isAdd);
        }

        internal void updateShield(string flagName, bool isAdd)
        {
            _logger.LogInformation("updateShield: {Name} | {Value}", flagName, isAdd);
            if ("shield-buckler".Equals(flagName))
            {
                shield.ToggleState(isAdd, 3);
            }
            else if ("shield-silver".Equals(flagName))
            {
                shield.ToggleState(isAdd, 1);
            }
            else if ("shield-fake".Equals(flagName))
            {
                shield.ToggleState(isAdd, 2);
            }
            else if ("shield-angel".Equals(flagName))
            {
                shield.ToggleState(isAdd, 0);
            }
        }

        internal void UpdateLampOfTime(string displayname, bool isAdd)
        {
            _logger.LogInformation("updateLampOfTime: {Name} | {Value}", displayname, isAdd);
            if ("invus-lamp-lit".Equals(displayname))
            {
                lampOfTime.ToggleState(isAdd, 0);
            }
        }

        internal void UpdateTranslationTablets(byte cur)
        {
            _logger.LogInformation("updateTranslationTables: {Value}", cur);
            if(cur == 3)
            {
                readerPanel.UpdateCount(100);
            }
            else if (cur == 2)
            {
                readerPanel.UpdateCount(60);
            }
            else if (cur == 1)
            {
                readerPanel.UpdateCount(20);
            }
            else if (cur == 0)
            {
                readerPanel.UpdateCount(0);
            }
        }

        public void SetGameStarted(bool started)
        {
            gameStarted = started;
            _trackerService.SetInGame(started);
        }

        public List<string> LastItems { get; set; } = new();
        
        internal void UpdateLastItem(string flagName)
        {
            if(gameStarted)
            {
                _logger.LogInformation("updateLastItem: {Name}", flagName);
                LastItems.Insert(0, flagName);
                if (LastItems.Count > 3)
                {
                    LastItems.RemoveRange(3, 4 - LastItems.Count);
                }
                lastItemPanel.Invoke(new Action(() =>
                {
                    System.Drawing.Bitmap? lastItemImage = getFoundImage(flagName);
                    if (lastItemImage != null)
                    {
                        System.Drawing.Image lastItemImageTemp = lastItem2.Image;
                        if (lastItemImageTemp != null)
                        {
                            lastItem3.Image = lastItemImageTemp;
                            lastItem3.BackgroundImage = lastItem2.BackgroundImage;
                            lastItem2.Image = lastItem1.Image;
                            lastItem2.BackgroundImage = lastItem1.BackgroundImage;
                        }
                        else
                        {
                            lastItemImageTemp = lastItem1.Image;
                            if (lastItemImageTemp != null)
                            {
                                lastItem2.Image = lastItemImageTemp;
                                lastItem2.BackgroundImage = lastItem1.BackgroundImage;
                            }
                        }

                        lastItem1.Image = lastItemImage;
                        if ("w-map-shrine".Equals(flagName))
                        {
                            lastItem1.BackgroundImage = Properties.Resources.Icon_map;
                        }
                        else
                        {
                            lastItem1.BackgroundImage = null;
                        }
                        lastItem1.Refresh();
                        lastItem2.Refresh();
                        lastItem3.Refresh();
                    }
                }));
                if (flagName.StartsWith("shield-"))
                {
                    _trackerService.SetItemState(flagName, true);    
                }
            }
        }

        public void toggleBoss(string itemName, bool isAdd)
        {
            _logger.LogInformation("toggleBoss: {Name} | {Value}", itemName, isAdd);
            if ("boss-amphisbaena".Equals(itemName))
            {
                amphisbaena.ToggleState(isAdd);
            }
            else if ("boss-sakit".Equals(itemName))
            {
                sakit.ToggleState(isAdd);
            }
            else if ("boss-ellmac".Equals(itemName))
            {
                ellmac.ToggleState(isAdd);
            }
            else if ("boss-bahamut".Equals(itemName))
            {
                bahamut.ToggleState(isAdd);
            }
            else if ("boss-viy".Equals(itemName))
            {
                viy.ToggleState(isAdd);
            }
            else if ("boss-palenque".Equals(itemName))
            {
                palenque.ToggleState(isAdd);
            }
            else if ("boss-baphomet".Equals(itemName))
            {
                baphomet.ToggleState(isAdd);
            }
            else if ("boss-tiamat".Equals(itemName))
            {
                tiamat.ToggleState(isAdd);
            }
            _trackerService.SetBossState(itemName, isAdd);
        }

        public void toggleMiniboss(string itemName, bool isAdd)
        {
            _logger.LogInformation("toggleMiniboss: {Name} | {Value}", itemName, isAdd);
            _trackerService.SetBossState(itemName, isAdd);
        }

        public void ToggleWhip(Boolean isAdd)
        {
            whip.ToggleState(isAdd, 2);
        }

        public void ToggleMap(string flagName, Boolean isAdd)
        {
            _logger.LogInformation("toggleMap: {Name} | {Value}", flagName, isAdd);
            mapsPanel.UpdateCount(isAdd);
            if ("w-map-shrine".Equals(flagName))
            {
                shrinePanel.UpdateCount(isAdd);
                maps.ToggleForeState(isAdd);
            }
            _trackerService.SetItemState(flagName, isAdd);
        }

        public void ToggleMantra(string flagName, bool isAdd)
        {
            _logger.LogInformation("toggleMantra: {Name} | {Value}", flagName, isAdd);
            if ("mantra-keysword".Equals(flagName))
            {
                keySword.ToggleState(isAdd, false);
            }
            else if ("mantra-amphisbaena".Equals(flagName))
            {
                amphisbaena.ToggleForeState(isAdd);
            }
            else if ("mantra-sakit".Equals(flagName))
            {
                sakit.ToggleForeState(isAdd);
            }
            else if ("mantra-ellmac".Equals(flagName))
            {
                ellmac.ToggleForeState(isAdd);
            }
            else if ("mantra-bahamut".Equals(flagName))
            {
                bahamut.ToggleForeState(isAdd);
            }
            else if ("mantra-viy".Equals(flagName))
            {
                viy.ToggleForeState(isAdd);
            }
            else if ("mantra-palenque".Equals(flagName))
            {
                palenque.ToggleForeState(isAdd);
            }
            else if ("mantra-baphomet".Equals(flagName))
            {
                baphomet.ToggleForeState(isAdd);
            }
            else if ("mantra-tiamat".Equals(flagName))
            {
                tiamat.ToggleForeState(isAdd);
            }
            _trackerService.SetItemState(flagName, isAdd);
        }

        public void SetAmmoCount(string flagName, int newCount)
        {
            _logger.LogInformation("setAmmoCount: {Name} | {Value}", flagName, newCount);
            if ("ammo-shuriken".Equals(flagName))
            {
                shurikenPanel.UpdateCount(newCount);
            }
            else if ("ammo-roll-shuriken".Equals(flagName))
            {
                rollingShurikenPanel.UpdateCount(newCount);
            }
            else if ("ammo-spear".Equals(flagName))
            {
                earthSpearPanel.UpdateCount(newCount);
            }
            else if ("ammo-flare".Equals(flagName))
            {
                flareGunPanel.UpdateCount(newCount);
            }
            else if ("ammo-bomb".Equals(flagName))
            {
                bombPanel.UpdateCount(newCount);
            }
            else if ("ammo-chakram".Equals(flagName))
            {
                chakramPanel.UpdateCount(newCount);
            }
            else if ("ammo-caltrop".Equals(flagName))
            {
                caltropsPanel.UpdateCount(newCount);
            }
            else if ("ammo-clip".Equals(flagName))
            {
                pistolPanel.UpdateCount(newCount, true);
            }
            else if ("ammo-bullet".Equals(flagName))
            {
                pistolPanel.UpdateCount(newCount, false);
            }
            else if ("ankh-jewels".Equals(flagName) && ankhJewelPanel.Item != null)
            {
                ankhJewelPanel.Item.Collected = newCount != 0;
                ankhJewelPanel.UpdateCount(newCount);
                _trackerService.SetItemCount(flagName, newCount);
            }
        }

        public void UpdateDeathCount(bool isAdd)
        {
            _logger.LogInformation("updateDeathCount: {Value}", isAdd);
            if (isAdd)
            {
                Properties.Settings.Default.DeathCount += 1;
                _trackerService.AddDeath(Properties.Settings.Default.DeathCount);
            }
            else
            {
                Properties.Settings.Default.DeathCount -= 1;
            }
            UpdateCount(deathCount, Properties.Settings.Default.DeathCount, int.MaxValue);
        }

        public void UpdateRegion(string region, bool isCleared)
        {

            if (isCleared)
            {
                _regionLabels[region].Invoke(() =>
                {
                    _regionLabels[region].Font = new System.Drawing.Font("Arial", 9, System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Strikeout);
                });
                
            }
            else
            {
                _regionLabels[region].Invoke(() =>
                {
                    _regionLabels[region].Font = new System.Drawing.Font("Arial", 9, System.Drawing.FontStyle.Bold);
                });
            }
        }

        private void LaMulanaItemTrackerForm_Load(object sender, EventArgs e)
        {
            if(Properties.Settings.Default.UpgradeRequired)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.UpgradeRequired = false;

                // Upgrade settings from 1.0.11 where item names were different.
                Properties.Settings.Default.Panel1Contents = Properties.Settings.Default.Panel1Contents.Replace("Hermes Boots", "Hermes' Boots").Replace("Glyph Reader", "reader.exe");
                Properties.Settings.Default.Panel2Contents = Properties.Settings.Default.Panel2Contents.Replace("Hermes Boots", "Hermes' Boots").Replace("Glyph Reader", "reader.exe");
                Properties.Settings.Default.Panel3Contents = Properties.Settings.Default.Panel3Contents.Replace("Hermes Boots", "Hermes' Boots").Replace("Glyph Reader", "reader.exe");
                Properties.Settings.Default.Panel4Contents = Properties.Settings.Default.Panel4Contents.Replace("Hermes Boots", "Hermes' Boots").Replace("Glyph Reader", "reader.exe");
                Properties.Settings.Default.Panel5Contents = Properties.Settings.Default.Panel5Contents.Replace("Hermes Boots", "Hermes' Boots").Replace("Glyph Reader", "reader.exe");
                Properties.Settings.Default.Panel6Contents = Properties.Settings.Default.Panel6Contents.Replace("Hermes Boots", "Hermes' Boots").Replace("Glyph Reader", "reader.exe");
                Properties.Settings.Default.Save();
            }
            ScaleImages(this);

            InitializePossibleItems();
            InitializeMenuOptions();

            foreach (var region in _config.Regions.Regions)
            {
                Label regionLabel = new Label();
                regionLabel.Text = region.Key;
                regionLabel.Margin = new Padding(0, 0, 0, 0);
                regionLabel.Size = new System.Drawing.Size(160, 16);
                regionLabel.Font = new System.Drawing.Font("Arial", 9, System.Drawing.FontStyle.Bold);
                regionsFlowPanel.Controls.Add(regionLabel);
                _regionLabels.Add(region.Key, regionLabel);
            }

            UpdateAlwaysOnTop();
            UpdateFormSize();
            UpdateFormColor();
            UpdateTextColor();
            UpdateBackgroundMode();
            UpdateShowLastItem();
            UpdateShowRegions();
            UpdateShowDeathCount();
            InitializeFormPanels();

            try
            {
                this._assembly = Assembly.GetExecutingAssembly();
                this._xmlStream = _assembly.GetManifestResourceStream("LMRItemTracker.names.xml");
            }
            catch
            {
                MessageBox.Show("Error accessing resources!");
            }

            UpdateVoiceTracker();
            UpdateRandomizerSettings();
            UpdateChatSettings();

            InitializeBackgroundWorker();
            flagListener.RunWorkerAsync();

            _chatModule.ContentUpdated += ChatModule_ContentUpdated;
        }

        private void ChatModule_ContentUpdated(object sender, EventArgs e)
        {
            UpdateCount(labelContentCount, _chatModule.Content, 100);
        }

        private void SelectFormColor(object sender, EventArgs e)
        {
            DialogOpen = true;
            if (formColorDialog.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default.BackgroundColor = formColorDialog.Color;
                UpdateFormColor();
            }
            DialogOpen = false;
            Redraw();
        }

        private void SelectTextColor(object sender, EventArgs e)
        {
            DialogOpen = true;
            if (textColorDialog.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default.TextColor = textColorDialog.Color;
                UpdateTextColor();
            }
            DialogOpen = false;
            Redraw();
        }

        private void SelectItemColor(object sender, EventArgs e)
        {
            DialogOpen = true;
            if (itemColorDialog.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default.ItemColor = itemColorDialog.Color;
                if("solid".Equals(Properties.Settings.Default.BackgroundMode))
                {
                    foreach (String item in allItems)
                    {
                        Control? control = GetControl(item);
                        if (control != null)
                        {
                            if (control is TrackerBox)
                            {
                                if(control.InvokeRequired)
                                {
                                    control.Invoke(new Action(() =>
                                    {
                                        control.Refresh();
                                    }));
                                }
                                else
                                {
                                    control.Refresh();
                                }
                            }
                        }
                    }
                }
            }
            DialogOpen = false;
            Redraw();
        }

        private void AddItemToPanel1(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem)
            {
                String itemName = ((ToolStripMenuItem)sender).Text;
                Control? control = GetControl(itemName);
                if (control == null)
                {
                    MessageBox.Show("Problem adding " + itemName + " to panel");
                }
                else
                {
                    control.TabIndex = flowLayoutPanel1.Controls.Count;
                    flowLayoutPanel1.Controls.Add(control);
                    control.Margin = new Padding(0);
                    flowLayoutPanel1.Visible = true;

                    Properties.Settings.Default.Panel1Contents = RebuildPanelContents(Properties.Settings.Default.Panel1Contents, itemName, true);

                    Properties.Settings.Default.Panel2Contents = RebuildPanelContents(Properties.Settings.Default.Panel2Contents, itemName, false);
                    Properties.Settings.Default.Panel3Contents = RebuildPanelContents(Properties.Settings.Default.Panel3Contents, itemName, false);
                    Properties.Settings.Default.Panel4Contents = RebuildPanelContents(Properties.Settings.Default.Panel4Contents, itemName, false);
                    Properties.Settings.Default.Panel5Contents = RebuildPanelContents(Properties.Settings.Default.Panel5Contents, itemName, false);
                    Properties.Settings.Default.Panel6Contents = RebuildPanelContents(Properties.Settings.Default.Panel6Contents, itemName, false);

                    Redraw();
                }
            }
        }

        private void AddItemToPanel2(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem)
            {
                String itemName = ((ToolStripMenuItem)sender).Text;
                Control? control = GetControl(itemName);
                if (control == null)
                {
                    MessageBox.Show("Problem adding " + itemName + " to panel");
                }
                else
                {
                    control.TabIndex = flowLayoutPanel2.Controls.Count;
                    flowLayoutPanel2.Controls.Add(control);
                    control.Margin = new Padding(0);
                    flowLayoutPanel2.Visible = true;

                    Properties.Settings.Default.Panel2Contents = RebuildPanelContents(Properties.Settings.Default.Panel2Contents, itemName, true);

                    Properties.Settings.Default.Panel1Contents = RebuildPanelContents(Properties.Settings.Default.Panel1Contents, itemName, false);
                    Properties.Settings.Default.Panel3Contents = RebuildPanelContents(Properties.Settings.Default.Panel3Contents, itemName, false);
                    Properties.Settings.Default.Panel4Contents = RebuildPanelContents(Properties.Settings.Default.Panel4Contents, itemName, false);
                    Properties.Settings.Default.Panel5Contents = RebuildPanelContents(Properties.Settings.Default.Panel5Contents, itemName, false);
                    Properties.Settings.Default.Panel6Contents = RebuildPanelContents(Properties.Settings.Default.Panel6Contents, itemName, false);

                    Redraw();
                }
            }
        }

        private void AddItemToPanel3(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem)
            {
                String itemName = ((ToolStripMenuItem)sender).Text;
                Control? control = GetControl(itemName);
                if (control == null)
                {
                    MessageBox.Show("Problem adding " + itemName + " to panel");
                }
                else
                {
                    control.TabIndex = flowLayoutPanel3.Controls.Count;
                    flowLayoutPanel3.Controls.Add(control);
                    control.Margin = new Padding(0);
                    flowLayoutPanel3.Visible = true;

                    Properties.Settings.Default.Panel3Contents = RebuildPanelContents(Properties.Settings.Default.Panel3Contents, itemName, true);

                    Properties.Settings.Default.Panel1Contents = RebuildPanelContents(Properties.Settings.Default.Panel1Contents, itemName, false);
                    Properties.Settings.Default.Panel2Contents = RebuildPanelContents(Properties.Settings.Default.Panel2Contents, itemName, false);
                    Properties.Settings.Default.Panel4Contents = RebuildPanelContents(Properties.Settings.Default.Panel4Contents, itemName, false);
                    Properties.Settings.Default.Panel5Contents = RebuildPanelContents(Properties.Settings.Default.Panel5Contents, itemName, false);
                    Properties.Settings.Default.Panel6Contents = RebuildPanelContents(Properties.Settings.Default.Panel6Contents, itemName, false);

                    Redraw();
                }
            }
        }

        private void AddItemToPanel4(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem)
            {
                String itemName = ((ToolStripMenuItem)sender).Text;
                Control? control = GetControl(itemName);
                if (control == null)
                {
                    MessageBox.Show("Problem adding " + itemName + " to panel");
                }
                else
                {
                    control.TabIndex = flowLayoutPanel4.Controls.Count;
                    flowLayoutPanel4.Controls.Add(control);
                    control.Margin = new Padding(0);
                    flowLayoutPanel4.Visible = true;

                    Properties.Settings.Default.Panel4Contents = RebuildPanelContents(Properties.Settings.Default.Panel4Contents, itemName, true);

                    Properties.Settings.Default.Panel1Contents = RebuildPanelContents(Properties.Settings.Default.Panel1Contents, itemName, false);
                    Properties.Settings.Default.Panel2Contents = RebuildPanelContents(Properties.Settings.Default.Panel2Contents, itemName, false);
                    Properties.Settings.Default.Panel3Contents = RebuildPanelContents(Properties.Settings.Default.Panel3Contents, itemName, false);
                    Properties.Settings.Default.Panel5Contents = RebuildPanelContents(Properties.Settings.Default.Panel5Contents, itemName, false);
                    Properties.Settings.Default.Panel6Contents = RebuildPanelContents(Properties.Settings.Default.Panel6Contents, itemName, false);

                    Redraw();
                }
            }
        }

        private void AddItemToPanel5(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem)
            {
                String itemName = ((ToolStripMenuItem)sender).Text;
                Control? control = GetControl(itemName);
                if (control == null)
                {
                    MessageBox.Show("Problem adding " + itemName + " to panel");
                }
                else
                {
                    control.TabIndex = flowLayoutPanel5.Controls.Count;
                    flowLayoutPanel5.Controls.Add(control);
                    control.Margin = new Padding(0);
                    flowLayoutPanel5.Visible = true;

                    Properties.Settings.Default.Panel5Contents = RebuildPanelContents(Properties.Settings.Default.Panel5Contents, itemName, true);

                    Properties.Settings.Default.Panel1Contents = RebuildPanelContents(Properties.Settings.Default.Panel1Contents, itemName, false);
                    Properties.Settings.Default.Panel2Contents = RebuildPanelContents(Properties.Settings.Default.Panel2Contents, itemName, false);
                    Properties.Settings.Default.Panel3Contents = RebuildPanelContents(Properties.Settings.Default.Panel3Contents, itemName, false);
                    Properties.Settings.Default.Panel4Contents = RebuildPanelContents(Properties.Settings.Default.Panel4Contents, itemName, false);
                    Properties.Settings.Default.Panel6Contents = RebuildPanelContents(Properties.Settings.Default.Panel6Contents, itemName, false);

                    Redraw();
                }
            }
        }

        private void AddItemToPanel6(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem)
            {
                String itemName = ((ToolStripMenuItem)sender).Text;
                Control? control = GetControl(itemName);
                if (control == null)
                {
                    MessageBox.Show("Problem adding " + itemName + " to panel");
                }
                else
                {
                    control.TabIndex = flowLayoutPanel6.Controls.Count;
                    flowLayoutPanel6.Controls.Add(control);
                    control.Margin = new Padding(0);
                    flowLayoutPanel6.Visible = true;

                    Properties.Settings.Default.Panel6Contents = RebuildPanelContents(Properties.Settings.Default.Panel6Contents, itemName, true);

                    Properties.Settings.Default.Panel1Contents = RebuildPanelContents(Properties.Settings.Default.Panel1Contents, itemName, false);
                    Properties.Settings.Default.Panel2Contents = RebuildPanelContents(Properties.Settings.Default.Panel2Contents, itemName, false);
                    Properties.Settings.Default.Panel3Contents = RebuildPanelContents(Properties.Settings.Default.Panel3Contents, itemName, false);
                    Properties.Settings.Default.Panel4Contents = RebuildPanelContents(Properties.Settings.Default.Panel4Contents, itemName, false);
                    Properties.Settings.Default.Panel5Contents = RebuildPanelContents(Properties.Settings.Default.Panel5Contents, itemName, false);

                    Redraw();
                }
            }
        }

        private void RemoveItemFromPanel(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem)
            {
                String itemName = ((ToolStripMenuItem)sender).Text;
                Control? control = GetControl(itemName);
                if (control == null)
                {
                    MessageBox.Show("Problem removing " + itemName + " from panel");
                }
                else
                {
                    Control parent = control.Parent;
                    control.Parent = null;
                    if(parent.Controls.Count < 1)
                    {
                        parent.Visible = false;
                    }
                    else
                    {
                        for (int index = 0; index < parent.Controls.Count; index++)
                        {
                            parent.Controls[index].TabIndex = index;
                        }
                    }
                    Properties.Settings.Default.Panel1Contents = RebuildPanelContents(Properties.Settings.Default.Panel1Contents, itemName, false);
                    Properties.Settings.Default.Panel2Contents = RebuildPanelContents(Properties.Settings.Default.Panel2Contents, itemName, false);
                    Properties.Settings.Default.Panel3Contents = RebuildPanelContents(Properties.Settings.Default.Panel3Contents, itemName, false);
                    Properties.Settings.Default.Panel4Contents = RebuildPanelContents(Properties.Settings.Default.Panel4Contents, itemName, false);
                    Properties.Settings.Default.Panel5Contents = RebuildPanelContents(Properties.Settings.Default.Panel5Contents, itemName, false);
                    Properties.Settings.Default.Panel6Contents = RebuildPanelContents(Properties.Settings.Default.Panel6Contents, itemName, false);

                    Redraw();
                }
            }
        }

        private System.Drawing.Bitmap? getFoundImage(string flagName)
        {
            if (flagName.StartsWith("w-orb-"))
            {
                return Properties.Resources.Icon_sacredorb;
            }
            else if ("w-map-shrine".Equals(flagName))
            {
                return Properties.Resources.Icon_dragonbone_small;
            }
            else if (flagName.StartsWith("w-map-"))
            {
                return Properties.Resources.Icon_map;
            }
            else if ("ankh-jewels".Equals(flagName))
            {
                return Properties.Resources.Icon_ankhjewel;
            }
            else if ("w-scanner".Equals(flagName))
            {
                return Properties.Resources.Icon_handscanner;
            }
            else if ("w-grail".Equals(flagName))
            {
                return Properties.Resources.Icon_holygrail;
            }
            else if ("w-doll".Equals(flagName))
            {
                return Properties.Resources.Icon_minidoll;
            }
            else if ("w-magatama".Equals(flagName))
            {
                return Properties.Resources.Icon_magatamajewel;
            }
            else if ("w-pepper".Equals(flagName))
            {
                return Properties.Resources.Icon_pepper;
            }
            else if ("w-woman".Equals(flagName))
            {
                return Properties.Resources.Icon_womanstatue;
            }
            else if ("w-serpent".Equals(flagName))
            {
                return Properties.Resources.Icon_serpentstaff;
            }
            else if ("w-glove".Equals(flagName))
            {
                return Properties.Resources.Icon_glove;
            }
            else if ("w-crucifix".Equals(flagName))
            {
                return Properties.Resources.Icon_crucifix;
            }
            else if ("w-eye-truth".Equals(flagName))
            {
                return Properties.Resources.Icon_eyeoftruth;
            }
            else if ("w-scale".Equals(flagName))
            {
                return Properties.Resources.Icon_scalesphere;
            }
            else if ("w-gauntlet".Equals(flagName))
            {
                return Properties.Resources.Icon_gauntlet;
            }
            else if ("w-anchor".Equals(flagName))
            {
                return Properties.Resources.Icon_anchor;
            }
            else if ("w-book".Equals(flagName))
            {
                return Properties.Resources.Icon_bookofthedead;
            }
            else if ("w-clothes".Equals(flagName))
            {
                return Properties.Resources.Icon_fairyclothes;
            }
            else if ("w-scriptures".Equals(flagName))
            {
                return Properties.Resources.Icon_scriptures;
            }
            else if ("w-bracelet".Equals(flagName))
            {
                return Properties.Resources.Icon_bracelet;
            }
            else if ("w-perfume".Equals(flagName))
            {
                return Properties.Resources.Icon_perfume;
            }
            else if ("w-spaulder".Equals(flagName))
            {
                return Properties.Resources.Icon_spaulder;
            }
            else if ("w-icecape".Equals(flagName))
            {
                return Properties.Resources.Icon_icecape;
            }
            else if ("w-talisman".Equals(flagName))
            {
                return Properties.Resources.Icon_talisman;
            }
            else if ("w-diary".Equals(flagName))
            {
                return Properties.Resources.Icon_diary;
            }
            else if ("w-mulanatalisman".Equals(flagName))
            {
                return Properties.Resources.Icon_mulanatalisman;
            }
            else if ("w-dimension-key".Equals(flagName))
            {
                return Properties.Resources.Icon_dimensionalkey;
            }
            else if ("w-djed".Equals(flagName))
            {
                return Properties.Resources.Icon_djedpillar;
            }
            else if ("w-cog".Equals(flagName))
            {
                return Properties.Resources.Icon_cogofthesoul;
            }
            else if ("w-dragonbone".Equals(flagName))
            {
                return Properties.Resources.Icon_dragonbone;
            }
            else if ("w-cskull".Equals(flagName))
            {
                return Properties.Resources.Icon_crystalskull;
            }
            else if ("w-endless-key".Equals(flagName))
            {
                return Properties.Resources.Icon_keyofeternity;
            }
            else if ("w-isispendant".Equals(flagName))
            {
                return Properties.Resources.Icon_isispendant;
            }
            else if ("w-helmet".Equals(flagName))
            {
                return Properties.Resources.Icon_helmet;
            }
            else if ("w-grapple".Equals(flagName))
            {
                return Properties.Resources.Icon_grappleclaw;
            }
            else if ("w-mirror".Equals(flagName))
            {
                return Properties.Resources.Icon_bronzemirror;
            }
            else if ("w-ring".Equals(flagName))
            {
                return Properties.Resources.Icon_ring;
            }
            else if ("w-plane".Equals(flagName))
            {
                return Properties.Resources.Icon_planemodel;
            }
            else if ("w-ocarina".Equals(flagName))
            {
                return Properties.Resources.Icon_philosophersocarina;
            }
            else if ("w-feather".Equals(flagName))
            {
                return Properties.Resources.Icon_feather;
            }
            else if ("w-hermes".Equals(flagName))
            {
                return Properties.Resources.Icon_hermesboots;
            }
            else if ("w-fruit".Equals(flagName))
            {
                return Properties.Resources.Icon_fruitofeden;
            }
            else if ("w-twin-statue".Equals(flagName))
            {
                return Properties.Resources.Icon_twinstatue;
            }
            else if ("w-treasures".Equals(flagName))
            {
                return Properties.Resources.Icon_treasures;
            }
            else if ("w-pochettekey".Equals(flagName))
            {
                return Properties.Resources.Icon_pochettekey;
            }
            else if ("w-msx2".Equals(flagName))
            {
                return Properties.Resources.Icon_msx2;
            }
            else if ("w-vessel".Equals(flagName))
            {
                return Properties.Resources.Icon_vessel;
            }
            else if ("w-water-case".Equals(flagName))
            {
                return Properties.Resources.Icon_waterproofcase;
            }
            else if ("w-heat-case".Equals(flagName))
            {
                return Properties.Resources.Icon_heatproofcase;
            }
            else if ("w-shell-horn".Equals(flagName))
            {
                return Properties.Resources.Icon_shellhorn;
            }
            else if ("w-main-chain".Equals(flagName))
            {
                return Properties.Resources.Icon_chainwhip;
            }
            else if ("w-main-flail".Equals(flagName))
            {
                return Properties.Resources.Icon_flailwhip;
            }
            else if ("w-main-axe".Equals(flagName))
            {
                return Properties.Resources.Icon_axe;
            }
            else if ("w-main-knife".Equals(flagName))
            {
                return Properties.Resources.Icon_knife;
            }
            else if ("w-main-katana".Equals(flagName))
            {
                return Properties.Resources.Icon_katana;
            }
            else if ("w-main-keysword".Equals(flagName))
            {
                return Properties.Resources.Icon_keysword;
            }
            else if ("w-sub-shuriken".Equals(flagName))
            {
                return Properties.Resources.Icon_shuriken;
            }
            else if ("w-sub-rshuriken".Equals(flagName))
            {
                return Properties.Resources.Icon_rollingshuriken;
            }
            else if ("w-sub-caltrops".Equals(flagName))
            {
                return Properties.Resources.Icon_caltrops;
            }
            else if ("w-sub-spear".Equals(flagName))
            {
                return Properties.Resources.Icon_earthspear;
            }
            else if ("w-sub-flare".Equals(flagName))
            {
                return Properties.Resources.Icon_flaregun;
            }
            else if ("w-sub-bomb".Equals(flagName))
            {
                return Properties.Resources.Icon_bomb;
            }
            else if ("w-sub-chakram".Equals(flagName))
            {
                return Properties.Resources.Icon_chakram;
            }
            else if ("w-sub-pistol".Equals(flagName))
            {
                return Properties.Resources.Icon_pistol;
            }
            else if ("w-seal1".Equals(flagName))
            {
                return Properties.Resources.Icon_originseal;
            }
            else if ("w-seal2".Equals(flagName))
            {
                return Properties.Resources.Icon_birthseal;
            }
            else if ("w-seal3".Equals(flagName))
            {
                return Properties.Resources.Icon_lifeseal;
            }
            else if ("w-seal4".Equals(flagName))
            {
                return Properties.Resources.Icon_deathseal;
            }
            else if ("w-soft-reader".Equals(flagName))
            {
                return Properties.Resources.Icon_reader;
            }
            else if ("w-soft-mantra".Equals(flagName))
            {
                return Properties.Resources.Icon_mantra;
            }
            else if ("w-soft-torude".Equals(flagName))
            {
                return Properties.Resources.Icon_torude;
            }
            else if ("w-soft-mekuri".Equals(flagName))
            {
                return Properties.Resources.Icon_mekuri;
            }
            else if ("w-soft-miracle".Equals(flagName))
            {
                return Properties.Resources.Icon_miracle;
            }
            else if ("w-soft-mirai".Equals(flagName))
            {
                return Properties.Resources.Icon_mirai;
            }
            else if ("w-soft-yagomap".Equals(flagName))
            {
                return Properties.Resources.Icon_yagomap;
            }
            else if ("w-soft-yagostr".Equals(flagName))
            {
                return Properties.Resources.Icon_yagostr;
            }
            else if ("w-soft-xmailer".Equals(flagName))
            {
                return Properties.Resources.Icon_xmailer;
            }
            else if ("w-soft-bunemon".Equals(flagName))
            {
                return Properties.Resources.Icon_bunemon;
            }
            else if ("w-soft-bunplus".Equals(flagName))
            {
                return Properties.Resources.Icon_bunplus;
            }
            else if ("w-soft-guild".Equals(flagName))
            {
                return Properties.Resources.Icon_guild;
            }
            else if ("w-soft-emusic".Equals(flagName))
            {
                return Properties.Resources.Icon_emusic;
            }
            else if ("w-soft-beolamu".Equals(flagName))
            {
                return Properties.Resources.Icon_beolamu;
            }
            else if ("w-soft-randc".Equals(flagName))
            {
                return Properties.Resources.Icon_randc;
            }
            else if ("w-soft-deathv".Equals(flagName))
            {
                return Properties.Resources.Icon_deathv;
            }
            else if ("w-soft-capstar".Equals(flagName))
            {
                return Properties.Resources.Icon_capstar;
            }
            else if ("w-soft-move".Equals(flagName))
            {
                return Properties.Resources.Icon_move;
            }
            else if ("w-soft-bounce".Equals(flagName))
            {
                return Properties.Resources.Icon_bounce;
            }
            else if ("w-soft-lamulana".Equals(flagName))
            {
                return Properties.Resources.Icon_lamulana;
            }
            else if ("shield-buckler".Equals(flagName))
            {
                return Properties.Resources.Icon_buckler;
            }
            else if ("shield-silver".Equals(flagName))
            {
                return Properties.Resources.Icon_silvershield;
            }
            else if ("shield-fake".Equals(flagName))
            {
                return Properties.Resources.Icon_silvershield2;
            }
            else if ("shield-angel".Equals(flagName))
            {
                return Properties.Resources.Icon_angelshield;
            }
            else if ("w-lamp".Equals(flagName))
            {
                return Properties.Resources.Icon_lampoftime;
            }
            else if ("w-forbidden".Equals(flagName))
            {
                return Properties.Resources.Icon_swimsuit;
            }
            else if ("whip".Equals(flagName))
            {
                return Properties.Resources.Icon_whip;
            }
            return null;
        }

        private String? getFlagName(string itemName)
        {
            if ("Hand Scanner".Equals(itemName))
            {
                return "w-scanner";
            }
            else if ("Holy Grail".Equals(itemName))
            {
                return "w-grail";
            }
            else if ("Mini Doll".Equals(itemName))
            {
                return "w-doll";
            }
            else if ("Magatama Jewel".Equals(itemName))
            {
                return "w-magatama";
            }
            else if ("Pepper".Equals(itemName))
            {
                return "w-pepper";
            }
            else if ("Woman Statue".Equals(itemName))
            {
                return "w-woman";
            }
            else if ("Serpent Staff".Equals(itemName))
            {
                return "w-serpent";
            }
            else if ("Glove".Equals(itemName))
            {
                return "w-glove";
            }
            else if ("Crucifix".Equals(itemName))
            {
                return "w-crucifix";
            }
            else if ("Eye of Truth".Equals(itemName))
            {
                return "w-eye-truth";
            }
            else if ("Scalesphere".Equals(itemName))
            {
                return "w-scale";
            }
            else if ("Gauntlet".Equals(itemName))
            {
                return "w-gauntlet";
            }
            else if ("Anchor".Equals(itemName))
            {
                return "w-anchor";
            }
            else if ("Book of the Dead".Equals(itemName))
            {
                return "w-book";
            }
            else if ("Fairy Clothes".Equals(itemName))
            {
                return "w-clothes";
            }
            else if ("Scriptures".Equals(itemName))
            {
                return "w-scriptures";
            }
            else if ("Bracelet".Equals(itemName))
            {
                return "w-bracelet";
            }
            else if ("Perfume".Equals(itemName))
            {
                return "w-perfume";
            }
            else if ("Spaulder".Equals(itemName))
            {
                return "w-spaulder";
            }
            else if ("Ice Cape".Equals(itemName))
            {
                return "w-icecape";
            }
            else if ("Talisman".Equals(itemName))
            {
                return "w-talisman";
            }
            else if ("Diary".Equals(itemName))
            {
                return "w-diary";
            }
            else if ("Mulana Talisman".Equals(itemName))
            {
                return "w-mulanatalisman";
            }
            else if ("Dimensional Key".Equals(itemName))
            {
                return "w-dimension-key";
            }
            else if ("Djed Pillar".Equals(itemName))
            {
                return "w-djed";
            }
            else if ("Cog of the Soul".Equals(itemName))
            {
                return "w-cog";
            }
            else if ("Dragon Bone".Equals(itemName))
            {
                return "w-dragonbone";
            }
            else if ("Crystal Skull".Equals(itemName))
            {
                return "w-cskull";
            }
            else if ("Key of Eternity".Equals(itemName))
            {
                return "w-endless-key";
            }
            else if ("Isis' Pendant".Equals(itemName))
            {
                return "w-isispendant";
            }
            else if ("Helmet".Equals(itemName))
            {
                return "w-helmet";
            }
            else if ("Grapple Claw".Equals(itemName))
            {
                return "w-grapple";
            }
            else if ("Bronze Mirror".Equals(itemName))
            {
                return "w-mirror";
            }
            else if ("Ring".Equals(itemName))
            {
                return "w-ring";
            }
            else if ("Plane Model".Equals(itemName))
            {
                return "w-plane";
            }
            else if ("Philosopher's Ocarina".Equals(itemName))
            {
                return "w-ocarina";
            }
            else if ("Feather".Equals(itemName))
            {
                return "w-feather";
            }
            else if ("Hermes' Boots".Equals(itemName))
            {
                return "w-hermes";
            }
            else if ("Fruit of Eden".Equals(itemName))
            {
                return "w-fruit";
            }
            else if ("Twin Statue".Equals(itemName))
            {
                return "w-twin-statue";
            }
            else if ("Treasures".Equals(itemName))
            {
                return "w-treasures";
            }
            else if ("Pochette Key".Equals(itemName))
            {
                return "w-pochettekey";
            }
            else if ("Mobile Super X2".Equals(itemName))
            {
                return "w-msx2";
            }
            else if ("Vessel/Medicine".Equals(itemName))
            {
                return "w-vessel";
            }
            else if ("Lamp of Time".Equals(itemName))
            {
                return "w-lamp";
            }
            else if ("Ankh Jewels".Equals(itemName))
            {
                return "ankh-jewels";
            }
            else if ("Waterproof Case".Equals(itemName))
            {
                return "w-water-case";
            }
            else if ("Heatproof Case".Equals(itemName))
            {
                return "w-heat-case";
            }
            else if ("Shell Horn".Equals(itemName))
            {
                return "w-shell-horn";
            }
            else if ("Whip".Equals(itemName))
            {
                return "whip";
            }
            else if ("Axe".Equals(itemName))
            {
                return "w-main-axe";
            }
            else if ("Knife".Equals(itemName))
            {
                return "w-main-knife";
            }
            else if ("Katana".Equals(itemName))
            {
                return "w-main-katana";
            }
            else if ("Key Sword".Equals(itemName))
            {
                return "w-main-keysword";
            }
            else if ("Shuriken".Equals(itemName))
            {
                return "w-sub-shuriken";
            }
            else if ("Rolling Shuriken".Equals(itemName))
            {
                return "w-sub-rshuriken";
            }
            else if ("Caltrops".Equals(itemName))
            {
                return "w-sub-caltrops";
            }
            else if ("Earth Spear".Equals(itemName))
            {
                return "w-sub-spear";
            }
            else if ("Flare Gun".Equals(itemName))
            {
                return "w-sub-flare";
            }
            else if ("Bomb".Equals(itemName))
            {
                return "w-sub-bomb";
            }
            else if ("Chakram".Equals(itemName))
            {
                return "w-sub-chakram";
            }
            else if ("Pistol".Equals(itemName))
            {
                return "w-sub-pistol";
            }
            else if ("Origin Seal".Equals(itemName))
            {
                return "w-seal1";
            }
            else if ("Birth Seal".Equals(itemName))
            {
                return "w-seal2";
            }
            else if ("Life Seal".Equals(itemName))
            {
                return "w-seal3";
            }
            else if ("Death Seal".Equals(itemName))
            {
                return "w-seal4";
            }
            else if ("reader.exe".Equals(itemName))
            {
                return "w-soft-reader";
            }
            else if ("mantra.exe".Equals(itemName))
            {
                return "w-soft-mantra";
            }
            else if ("Mantra/Djed Pillar".Equals(itemName))
            {
                return "w-soft-mantra";
            }
            else if ("torude.exe".Equals(itemName))
            {
                return "w-soft-torude";
            }
            else if ("mekuri.exe".Equals(itemName))
            {
                return "w-soft-mekuri";
            }
            else if ("miracle.exe".Equals(itemName))
            {
                return "w-soft-miracle";
            }
            else if ("mirai.exe".Equals(itemName))
            {
                return "w-soft-mirai";
            }
            else if ("yagomap.exe".Equals(itemName))
            {
                return "w-soft-yagomap";
            }
            else if ("yagostr.exe".Equals(itemName))
            {
                return "w-soft-yagostr";
            }
            else if ("xmailer.exe".Equals(itemName))
            {
                return "w-soft-xmailer";
            }
            else if ("bunemon.exe".Equals(itemName))
            {
                return "w-soft-bunemon";
            }
            else if ("bunplus.com".Equals(itemName))
            {
                return "w-soft-bunplus";
            }
            else if ("guild.exe".Equals(itemName))
            {
                return "w-soft-guild";
            }
            else if ("emusic.exe".Equals(itemName))
            {
                return "w-soft-emusic";
            }
            else if ("beolamu.exe".Equals(itemName))
            {
                return "w-soft-beolamu";
            }
            else if ("randc.exe".Equals(itemName))
            {
                return "w-soft-randc";
            }
            else if ("deathv.exe".Equals(itemName))
            {
                return "w-soft-deathv";
            }
            else if ("capstar.exe".Equals(itemName))
            {
                return "w-soft-capstar";
            }
            else if ("move.exe".Equals(itemName))
            {
                return "w-soft-move";
            }
            else if ("bounce.exe".Equals(itemName))
            {
                return "w-soft-bounce";
            }
            else if ("lamulana.exe".Equals(itemName))
            {
                return "w-soft-lamulana";
            }
            else if ("Shield".Equals(itemName))
            {
                return "shield-buckler";
            }
            else if ("Buckler".Equals(itemName))
            {
                return "shield-buckler";
            }
            else if ("Silver Shield".Equals(itemName))
            {
                return "shield-silver";
            }
            else if ("Fake Silver Shield".Equals(itemName))
            {
                return "shield-fake";
            }
            else if ("Angel Shield".Equals(itemName))
            {
                return "shield-angel";
            }
            else if ("Provocative Bathing Suit".Equals(itemName))
            {
                return "w-forbidden";
            }
            return null;
        }

        public Control? GetControl(String itemName)
        {
            if ("Hermes' Boots".Equals(itemName))
            {
                return hermesBoots;
            }
            if ("Grapple Claw".Equals(itemName))
            {
                return grappleClaw;
            }
            if ("Feather".Equals(itemName))
            {
                return feather;
            }
            if ("Hand Scanner".Equals(itemName))
            {
                return scanner;
            }
            if ("mirai.exe".Equals(itemName))
            {
                return mirai;
            }
            if ("Bronze Mirror".Equals(itemName))
            {
                return bronzeMirror;
            }
            if ("Fruit of Eden".Equals(itemName))
            {
                return fruitOfEden;
            }
            if ("Twin Statue".Equals(itemName))
            {
                return twinStatue;
            }
            if ("Key of Eternity".Equals(itemName))
            {
                return keyOfEternity;
            }
            if ("Helmet".Equals(itemName))
            {
                return helmet;
            }
            if ("Plane Model".Equals(itemName))
            {
                return planeModel;
            }
            if ("Crystal Skull".Equals(itemName))
            {
                return crystalSkull;
            }
            if ("Dimensional Key".Equals(itemName))
            {
                return dimensionalKey;
            }
            if ("Djed Pillar".Equals(itemName))
            {
                return djedPillar;
            }
            if ("Pochette Key".Equals(itemName))
            {
                return pochetteKey;
            }
            if ("Ice Cape".Equals(itemName))
            {
                return iceCape;
            }
            if ("Scalesphere".Equals(itemName))
            {
                return scalesphere;
            }
            if ("Cog of the Soul".Equals(itemName))
            {
                return cogOfTheSoul;
            }
            if ("Dragon Bone".Equals(itemName))
            {
                return dragonBone;
            }
            if ("Serpent Staff".Equals(itemName))
            {
                return serpentStaff;
            }
            if ("Mulana Talisman".Equals(itemName))
            {
                return mulanaTalisman;
            }
            if ("Pepper".Equals(itemName))
            {
                return pepper;
            }
            if ("Talisman".Equals(itemName))
            {
                return talisman;
            }
            if ("Diary".Equals(itemName))
            {
                return diary;
            }
            if ("Mini Doll".Equals(itemName))
            {
                return miniDoll;
            }
            if ("Treasures".Equals(itemName))
            {
                return treasures;
            }
            if ("Anchor".Equals(itemName))
            {
                return anchor;
            }
            if ("Isis' Pendant".Equals(itemName))
            {
                return isisPendant;
            }
            if ("Eye of Truth".Equals(itemName))
            {
                return eyeOfTruth;
            }
            if ("Magatama Jewel".Equals(itemName))
            {
                return magatamaJewel;
            }
            if ("torude.exe".Equals(itemName))
            {
                return torude;
            }
            if ("Origin Seal".Equals(itemName))
            {
                return originSeal;
            }
            if ("Birth Seal".Equals(itemName))
            {
                return birthSeal;
            }
            if ("Life Seal".Equals(itemName))
            {
                return lifeSeal;
            }
            if ("Death Seal".Equals(itemName))
            {
                return deathSeal;
            }
            if ("Book of the Dead".Equals(itemName))
            {
                return bookOfTheDead;
            }
            if ("Ring".Equals(itemName))
            {
                return ring;
            }
            if ("Fairy Clothes".Equals(itemName))
            {
                return fairyClothes;
            }
            if ("Mobile Super X2".Equals(itemName))
            {
                return msx2;
            }
            if ("Scriptures".Equals(itemName))
            {
                return scriptures;
            }
            if ("Crucifix".Equals(itemName))
            {
                return crucifix;
            }
            if ("Perfume".Equals(itemName))
            {
                return perfume;
            }
            if ("Bracelet".Equals(itemName))
            {
                return bracelet;
            }
            if ("Glove".Equals(itemName))
            {
                return glove;
            }
            if ("Spaulder".Equals(itemName))
            {
                return spaulder;
            }
            if ("Knife".Equals(itemName))
            {
                return knife;
            }
            if ("Axe".Equals(itemName))
            {
                return axe;
            }
            if ("Katana".Equals(itemName))
            {
                return katana;
            }
            if ("Gauntlet".Equals(itemName))
            {
                return gauntlet;
            }
            if ("Philosopher's Ocarina".Equals(itemName))
            {
                return ocarina;
            }
            if ("Shell Horn".Equals(itemName))
            {
                return shellHorn;
            }
            if ("Waterproof Case".Equals(itemName))
            {
                return waterproofCase;
            }
            if ("Heatproof Case".Equals(itemName))
            {
                return heatproofCase;
            }
            if ("xmailer.exe".Equals(itemName))
            {
                return xmailer;
            }
            if ("mantra.exe".Equals(itemName))
            {
                return mantraSingle;
            }
            if ("yagomap.exe".Equals(itemName))
            {
                return yagomap;
            }
            if ("yagostr.exe".Equals(itemName))
            {
                return yagostr;
            }
            if ("bunemon.exe".Equals(itemName))
            {
                return bunemon;
            }
            if ("bunplus.com".Equals(itemName))
            {
                return bunplus;
            }
            if ("guild.exe".Equals(itemName))
            {
                return guild;
            }
            if ("emusic.exe".Equals(itemName))
            {
                return emusic;
            }
            if ("beolamu.exe".Equals(itemName))
            {
                return beolamu;
            }
            if ("deathv.exe".Equals(itemName))
            {
                return deathv;
            }
            if ("randc.exe".Equals(itemName))
            {
                return randc;
            }
            if ("capstar.exe".Equals(itemName))
            {
                return capstar;
            }
            if ("move.exe".Equals(itemName))
            {
                return move;
            }
            if ("mekuri.exe".Equals(itemName))
            {
                return mekuri;
            }
            if ("bounce.exe".Equals(itemName))
            {
                return bounce;
            }
            if ("miracle.exe".Equals(itemName))
            {
                return miracle;
            }
            if ("lamulana.exe".Equals(itemName))
            {
                return lamulana;
            }
            if ("Provocative Bathing Suit".Equals(itemName))
            {
                return swimsuit;
            }
            if ("reader.exe".Equals(itemName))
            {
                return readerPanel;
            }
            if ("Woman Statue".Equals(itemName))
            {
                return womanStatue;
            }
            if ("Key Fairy Combo".Equals(itemName))
            {
                return keyFairy;
            }
            if ("Shrine Wall Removal".Equals(itemName))
            {
                return shrinePanel;
            }
            if ("Holy Grail".Equals(itemName))
            {
                return holyGrail;
            }
            if ("Whip".Equals(itemName))
            {
                return whip;
            }
            if ("Shield".Equals(itemName))
            {
                return shield;
            }
            if ("Shuriken".Equals(itemName))
            {
                return shurikenPanel;
            }
            if ("Rolling Shuriken".Equals(itemName))
            {
                return rollingShurikenPanel;
            }
            if ("Caltrops".Equals(itemName))
            {
                return caltropsPanel;
            }
            if ("Flare Gun".Equals(itemName))
            {
                return flareGunPanel;
            }
            if ("Chakram".Equals(itemName))
            {
                return chakramPanel;
            }
            if ("Earth Spear".Equals(itemName))
            {
                return earthSpearPanel;
            }
            if ("Bomb".Equals(itemName))
            {
                return bombPanel;
            }
            if ("Pistol".Equals(itemName))
            {
                return pistolPanel;
            }
            if ("Buckler".Equals(itemName))
            {
                return shield;
            }
            if ("Mantra/Djed Pillar".Equals(itemName))
            {
                return mantra;
            }
            if ("Vessel/Medicine".Equals(itemName))
            {
                return vessel;
            }
            if ("Key Sword".Equals(itemName))
            {
                return keySword;
            }
            if ("Lamp of Time".Equals(itemName))
            {
                return lampOfTime;
            }
            if ("Maps".Equals(itemName))
            {
                return mapsPanel;
            }
            if ("Ankh Jewels".Equals(itemName))
            {
                return ankhJewelPanel;
            }
            return null;
        }

        private ToolStripMenuItem CreateMenuItem(String itemName, EventHandler? eventHandler)
        {
            ToolStripMenuItem menuItem = new ToolStripMenuItem();
            menuItem.Text = itemName;
            if (eventHandler != null)
            {
                menuItem.Click += eventHandler;
            }
            return menuItem;
        }

        private String RebuildPanelContents(String currentSettingString, String itemName, Boolean add)
        {
            List<String> itemsInPanel = new List<String>(currentSettingString.Split(','));
            itemsInPanel.Remove(itemName); // In case it's already there (re-adding will move an item to the end)
            if (add)
            {
                itemsInPanel.Add(itemName);
            }
            return String.Join(",", itemsInPanel);
        }

        private void UpdateFormSize()
        {
            this.Width = Properties.Settings.Default.FormWidth;
            this.Height = Properties.Settings.Default.FormHeight;
        }

        private void UpdateBackgroundMode()
        {
            if (Properties.Settings.Default.BackgroundMode.Equals("shaded"))
            {
                shadedImageToolStripMenuItem.Checked = true;
                solidImageToolStripMenuItem.Checked = false;
                noImageToolStripMenuItem.Checked = false;
                hideImageToolStripMenuItem.Checked = false;
            }
            else if (Properties.Settings.Default.BackgroundMode.Equals("solid"))
            {
                shadedImageToolStripMenuItem.Checked = false;
                solidImageToolStripMenuItem.Checked = true;
                noImageToolStripMenuItem.Checked = false;
                hideImageToolStripMenuItem.Checked = false;
            }
            else if (Properties.Settings.Default.BackgroundMode.Equals("blank"))
            {
                shadedImageToolStripMenuItem.Checked = false;
                solidImageToolStripMenuItem.Checked = false;
                noImageToolStripMenuItem.Checked = true;
                hideImageToolStripMenuItem.Checked = false;
            }
            else
            {
                shadedImageToolStripMenuItem.Checked = false;
                solidImageToolStripMenuItem.Checked = false;
                noImageToolStripMenuItem.Checked = false;
                hideImageToolStripMenuItem.Checked = true;
            }
        }

        private void UpdateShowLastItem()
        {
            if(Properties.Settings.Default.ShowLastItem)
            {
                if(lastItemPanel.Parent == null)
                {
                    overviewPanel.Controls.Add(lastItemPanel);
                }
            }
            else
            {
                lastItemPanel.Parent = null;
            }
            showLastItemToolStripMenuItem.Checked = Properties.Settings.Default.ShowLastItem;
        }

        private void UpdateShowRegions()
        {
            regionsLabel.Visible = Properties.Settings.Default.ShowRegions;
            regionsFlowPanel.Visible = Properties.Settings.Default.ShowRegions;
            showRegionsToolStripMenuItem.Checked = Properties.Settings.Default.ShowRegions;
        }

        private void UpdateAlwaysOnTop()
        {
            TopMost = Properties.Settings.Default.AlwaysOnTop;
            alwaysOnTopToolStripMenuItem.Checked = Properties.Settings.Default.AlwaysOnTop;
        }

        private void UpdateShowAmmoCount()
        {
            shurikenPanel.Redraw();
            rollingShurikenPanel.Redraw();
            caltropsPanel.Redraw();
            flareGunPanel.Redraw();
            chakramPanel.Redraw();
            earthSpearPanel.Redraw();
            bombPanel.Redraw();
            pistolPanel.Redraw();

            showAmmoCountToolStripMenuItem.Checked = Properties.Settings.Default.ShowAmmoCount;
        }

        private void UpdateShowDeathCount()
        {
            showDeathCountToolStripMenuItem.Checked = Properties.Settings.Default.ShowDeathCount;
            UpdateCount(deathCount, Properties.Settings.Default.DeathCount, int.MaxValue);
            deathPanel.Visible = Properties.Settings.Default.ShowDeathCount;
        }

        private void UpdateVoiceTracker()
        {
            enableVoiceTrackerToolStripMenuItem.Checked = Properties.Settings.Default.EnableVoiceTracker;
            if (Properties.Settings.Default.EnableVoiceTracker)
            {
                _trackerService.Enable();
            }
            else
            {
                _trackerService.Disable();
            }
        }

        private void UpdateRandomizerSettings()
        {
            _trackerService.UpdateRandomizerPath(Properties.Settings.Default.RandomizerPath);
            _voiceRecognitionService.UpdateThresholds(Properties.Settings.Default.RecognitionThreshold, Properties.Settings.Default.ExecutionThreshold);
        }

        private void UpdateChatSettings()
        {
            var twitchUserName = Properties.Settings.Default.TwitchUserName;
            var twitchChannel = Properties.Settings.Default.TwitchChannel;
            var twitchId = Properties.Settings.Default.TwitchId;
            var twitchAuthToken = Properties.Settings.Default.TwitchOAuthToken;
            var respondToChat = Properties.Settings.Default.EnableTwitchChatResponses;
            var enablePolls = Properties.Settings.Default.EnableTwitchPolls;
            _chatModule.SetTwitchData(twitchUserName, twitchAuthToken, twitchChannel, twitchId, respondToChat, enablePolls);
        }

        private void UpdateFormColor()
        {
            this.BackColor = Properties.Settings.Default.BackgroundColor;
        }

        private void UpdateTextColor()
        {
            lastItemLabel.ForeColor = Properties.Settings.Default.TextColor;
            deathLabel.ForeColor = Properties.Settings.Default.TextColor;
            deathCount.ForeColor = Properties.Settings.Default.TextColor;
            labelContent.ForeColor = Properties.Settings.Default.TextColor;
            labelContentCount.ForeColor = Properties.Settings.Default.TextColor;
            regionsLabel.ForeColor = Properties.Settings.Default.TextColor;

            mapCount.UpdateTextColor();
            ankhJewelCount.UpdateTextColor();
            translationTablets.UpdateTextColor();
            shurikenAmmoCount.UpdateTextColor();
            rollingShurikenAmmoCount.UpdateTextColor();
            caltropsAmmoCount.UpdateTextColor();
            flareGunAmmoCount.UpdateTextColor();
            chakramAmmoCount.UpdateTextColor();
            earthSpearAmmoCount.UpdateTextColor();
            bombAmmoCount.UpdateTextColor();
            pistolAmmoCount.UpdateTextColor();

            skullWallCount.UpdateTextColor();

            foreach (var label in _regionLabels.Values)
            {
                label.ForeColor = Properties.Settings.Default.TextColor;
            }
        }

        private void SaveSettings(object sender, EventArgs e)
        {
            Properties.Settings.Default.FormWidth = this.Width;
            Properties.Settings.Default.FormHeight = this.Height;
            Properties.Settings.Default.Save();
        }

        private void RestoreDefaultSettings(object sender, EventArgs e)
        {
            Properties.Settings.Default.Panel1Contents = "Hermes' Boots,Grapple Claw,Feather,Hand Scanner,reader.exe,Holy Grail,mirai.exe";
            Properties.Settings.Default.Panel2Contents = "Bronze Mirror,Fruit of Eden,Twin Statue,Key of Eternity,Helmet,Plane Model,Crystal Skull,Dimensional Key,Pochette Key,Ice Cape,Scalesphere,Cog of the Soul,Dragon Bone,Serpent Staff,Mulana Talisman,Woman Statue,Pepper,Talisman,Diary,Mini Doll,Treasures,Anchor,Key Fairy Combo,Isis' Pendant,Eye of Truth,Magatama Jewel,torude.exe,Shrine Wall Removal";
            Properties.Settings.Default.Panel3Contents = "Origin Seal,Birth Seal,Life Seal,Death Seal,Book of the Dead,Ring,Fairy Clothes,Mobile Super X2,Scriptures,Crucifix,Perfume,Glove,Bracelet,Spaulder";
            Properties.Settings.Default.Panel4Contents = "Whip,Knife,Axe,Katana,Shield,Gauntlet,Pistol,Shuriken,Rolling Shuriken,Caltrops,Flare Gun,Chakram,Earth Spear,Bomb";
            Properties.Settings.Default.Panel5Contents = "Philosopher's Ocarina,Mantra/Djed Pillar,Vessel/Medicine,Key Sword,Lamp of Time,Maps,Ankh Jewels";
            Properties.Settings.Default.BackgroundColor = System.Drawing.SystemColors.Control;
            Properties.Settings.Default.TextColor = System.Drawing.Color.FromArgb(70, 70, 200);
            Properties.Settings.Default.FormWidth = 356;
            Properties.Settings.Default.FormHeight = 822;
            Properties.Settings.Default.BackgroundMode = "shaded";
            Properties.Settings.Default.ShowLastItem = true;
            Properties.Settings.Default.AlwaysOnTop = true;
            Properties.Settings.Default.ShowAmmoCount = true;
            Properties.Settings.Default.ShowDeathCount = true;
            Properties.Settings.Default.DeathCount = 0;

            UpdateAlwaysOnTop();
            UpdateFormSize();
            UpdateFormColor();
            UpdateTextColor();
            UpdateShowLastItem();
            UpdateShowRegions();
            InitializeFormPanels();
            Redraw();
            Refresh();
        }

        private void SetImage(String flagName, bool isAdd)
        {
            String? itemName = getItemName(flagName);
            if (itemName == null)
            {
                return;
            }
            Control? control = GetControl(itemName);

            if (control is TrackerBox)
            {
                ((TrackerBox)control).ToggleState(isAdd);
            }
            else if (control is ItemTextPanel)
            {
                ((ItemTextPanel)control).ToggleState(isAdd);
            }
        }

        private void SetBackgroundImage(PictureBox pictureBox, System.Drawing.Image image)
        {
            if (pictureBox.InvokeRequired)
            {
                pictureBox.Invoke(new Action(() =>
                {
                    pictureBox.BackgroundImage = image;
                    pictureBox.Refresh();
                }));
            }
            else
            {
                pictureBox.BackgroundImage = image;
            }
        }

        private void UpdateCount(Label label, int newCount, int max)
        {
            if (newCount >= 0 && newCount <= max)
            {
                if (label.InvokeRequired)
                {
                    label.Invoke(new Action(() =>
                    {
                        label.Text = "" + newCount;
                        label.Refresh();
                    }));
                }
                else
                {
                    label.Text = "" + newCount;
                }
            }
        }

        private void UpdatePistolAmmoCount(Label label, bool clip, int newCount)
        {
            if (clip)
            {
                if (newCount >= 0 && newCount <= 3)
                {
                    if (label.InvokeRequired)
                    {
                        label.Invoke(new Action(() =>
                        {
                            label.Text = newCount + ":" + label.Text.Substring(label.Text.IndexOf(':'));
                            label.Refresh();
                        }));
                    }
                    else
                    {
                        label.Text = newCount + ":" + label.Text.Substring(label.Text.IndexOf(':'));
                    }
                }
            }
            else if (newCount >= 0 && newCount <= 6)
            {
                if (label.InvokeRequired)
                {
                    label.Invoke(new Action(() =>
                    {
                        label.Text = label.Text.Substring(0, label.Text.IndexOf(':')) + ":" + newCount;
                        label.Refresh();
                    }));
                }
                else
                {
                    label.Text = label.Text.Substring(0, label.Text.IndexOf(':')) + ":" + newCount;
                }
            }
        }

        private void toggleShowLastItem(object sender, EventArgs e)
        {
            Properties.Settings.Default.ShowLastItem = !Properties.Settings.Default.ShowLastItem;
            UpdateShowLastItem();
        }

        public void ClearRecentItems()
        {
            lastItem1.Invoke(new Action(() =>
            {
                lastItem1.Image = null;
                lastItem1.BackgroundImage = null;
                lastItem1.Refresh();
            }));
            lastItem2.Invoke(new Action(() =>
            {
                lastItem2.Image = null;
                lastItem2.BackgroundImage = null;
                lastItem2.Refresh();
            }));
            lastItem3.Invoke(new Action(() =>
            {
                lastItem3.Image = null;
                lastItem3.BackgroundImage = null;
                lastItem3.Refresh();
            }));
        }

        private void clearLastItem(object sender, EventArgs e)
        {
            ClearRecentItems();
        }

        private void changeLanguage(object sender, EventArgs e)
        {
            lastItemLabel.Invoke(new Action(() =>
            {
                if(lastItemLabel.Text.Equals("Recent Items:"))
                {
                    lastItemLabel.Text = "最近:";
                }
                else
                {
                    lastItemLabel.Text = "Recent Items:";
                }
                lastItemLabel.Refresh();
            }));
        }

        private void toggleTopMost(object sender, EventArgs e)
        {
            Properties.Settings.Default.AlwaysOnTop = !Properties.Settings.Default.AlwaysOnTop;
            UpdateAlwaysOnTop();
        }

        private void toggleAmmoCount(object sender, EventArgs e)
        {
            Properties.Settings.Default.ShowAmmoCount = !Properties.Settings.Default.ShowAmmoCount;
            UpdateShowAmmoCount();
            Refresh();
        }

        private void toggleDeathCount(object sender, EventArgs e)
        {
            Properties.Settings.Default.ShowDeathCount = !Properties.Settings.Default.ShowDeathCount;
            UpdateShowDeathCount();
        }

        private void toggleVoiceTracker(object sender, EventArgs e)
        {
            Properties.Settings.Default.EnableVoiceTracker = !Properties.Settings.Default.EnableVoiceTracker;
            UpdateVoiceTracker();
        }

        private void setHideUncollected(object sender, EventArgs e)
        {
            Properties.Settings.Default.BackgroundMode = "hide";
            Properties.Settings.Default.Save();
            UpdateBackgroundMode();
            Redraw();
        }

        private void setBlankUncollected(object sender, EventArgs e)
        {
            Properties.Settings.Default.BackgroundMode = "blank";
            Properties.Settings.Default.Save();
            UpdateBackgroundMode();
            Redraw();
        }

        private void setShadedUncollected(object sender, EventArgs e)
        {
            Properties.Settings.Default.BackgroundMode = "shaded";
            Properties.Settings.Default.Save();
            UpdateBackgroundMode();
            Redraw();
        }

        private void setSolidUncollected(object sender, EventArgs e)
        {
            Properties.Settings.Default.BackgroundMode = "solid";
            Properties.Settings.Default.Save();
            UpdateBackgroundMode();
            Redraw();
        }

        private void Redraw()
        {
            RedrawPanel(flowLayoutPanel1);
            RedrawPanel(flowLayoutPanel2);
            RedrawPanel(flowLayoutPanel3);
            RedrawPanel(flowLayoutPanel4);
            RedrawPanel(flowLayoutPanel5);
            RedrawPanel(flowLayoutPanel6);
        }

        private void RedrawPanel(FlowLayoutPanel flowLayoutPanel)
        {
            foreach (Control control in flowLayoutPanel.Controls)
            {
                if (control is TrackerBox)
                {
                    ((TrackerBox)control).Redraw();
                }
                else if (control is ItemTextPanel)
                {
                    ((ItemTextPanel)control).Redraw();
                }
                else if (control is PistolPanel)
                {
                    ((PistolPanel)control).Redraw();
                }
                else if (control is KeySwordTrackerBox)
                {
                    ((KeySwordTrackerBox)control).Redraw();
                }
                else if (control is KeyFairyTrackerBox)
                {
                    ((KeyFairyTrackerBox)control).Redraw();
                }
                else if (control is MultiStateTrackerBox)
                {
                    ((MultiStateTrackerBox)control).Redraw();
                }
            }
        }

        private void resetDeathCount(object sender, EventArgs e)
        {
            Properties.Settings.Default.DeathCount = 0;
            UpdateCount(deathCount, Properties.Settings.Default.DeathCount, int.MaxValue);
        }

        private void deathCount_MouseClick(object sender, MouseEventArgs e)
        {
            MouseEventArgs me = e;
            if (me.Button == MouseButtons.Left)
            {
                UpdateDeathCount(true);
            }
            else if (me.Button == MouseButtons.Right)
            {
                UpdateDeathCount(false);
            }
        }

        private void openSettingsWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = _services.GetRequiredService<TrackerSettingsForm>();
            form.BringToFront();
            form.TopMost = true;
            var result = form.ShowDialog();
            if (result == DialogResult.OK)
            {
                UpdateRandomizerSettings();
                UpdateChatSettings();
                Properties.Settings.Default.Save();
            }
        }

        private void connectToChatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _chatModule.Connect();
        }

        private void resetRegionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _trackerService.ResetRegions();
        }

        private void showRegionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.ShowRegions = !Properties.Settings.Default.ShowRegions;
            UpdateShowRegions();
        }
    }
}
