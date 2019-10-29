using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Engine;

namespace SuperAdventure
{
    public partial class SuoerAdventure : Form
    {
        private Player _player;

        private Monster _currentMonster;
        public SuoerAdventure()
        {
            InitializeComponent();

            _player = new Player(10, 10, 20, 0, 1);
            MoveTo(World.LocationByID(World.LOCATION_ID_HOME));
            _player.Inventory.Add(new InventoryItem(World.ItemByID(World.ITEM_ID_RUSTY_SWORD),1 ));

            lblHitPoints.Text = _player.CurrentHitPoints.ToString();
            lblGold.Text = _player.Gold.ToString();
            lblExperience.Text = _player.ExperiencePoints.ToString();
            lblLevel.Text = _player.Level.ToString();

        }

        private void BtnNorth_Click(object sender, EventArgs e)
        {
            MoveTo(_player.CurrentLocation.LocationToNorth);
        }

        private void BtnEast_Click(object sender, EventArgs e)
        {
            MoveTo(_player.CurrentLocation.LocationToEast);
        }

        private void BtnSouth_Click(object sender, EventArgs e)
        {
            MoveTo(_player.CurrentLocation.LocationToSouth);
        }

        private void BtnWest_Click(object sender, EventArgs e)
        {
            MoveTo(_player.CurrentLocation.LocationToWest);
        }

        private void MoveTo(Location newLocation)
        {
            if(newLocation.ItemRequiredToEnter != null)
            {
                bool playerHasRequiredItem = false;

                foreach (InventoryItem ii in _player.Inventory)
                {
                    if(ii.Details.ID == newLocation.ItemRequiredToEnter.ID)
                    {
                        playerHasRequiredItem = true;
                        break;
                    }
                }
                if(!playerHasRequiredItem)
                {
                    rtbMessages.Text += "You must have a " + newLocation.ItemRequiredToEnter.Name + " to enter this location. " + Environment.NewLine;
                    return;
                }
            }

            _player.CurrentLocation = newLocation;

            btnNorth.Visible = (newLocation.LocationToNorth != null);
            btnEast.Visible = (newLocation.LocationToEast != null);
            btnSouth.Visible = (newLocation.LocationToSouth != null);
            btnWest.Visible = (newLocation.LocationToWest != null);

            rtbLocation.Text = newLocation.Name + Environment.NewLine;
            rtbLocation.Text = newLocation.Description + Environment.NewLine;

            _player.CurrentHitPoints = _player.MaximumHitPoints;

            lblHitPoints.Text = _player.CurrentHitPoints.ToString();


            if(newLocation.QuestAvailableHere != null)
            {
                bool playerAlreadyHasQuest = false;
                bool playerAlreadyCompletedQuest = false;

                foreach(PlayerQuest playerQuest in _player.Quests)
                {
                    if(playerQuest.Details.ID == newLocation.QuestAvailableHere.ID)
                    {
                        playerAlreadyHasQuest = true;

                        if(playerQuest.IsCompleted)
                        {
                            playerAlreadyCompletedQuest = true;
                        }
                    }
                }

                if(playerAlreadyHasQuest)
                {
                    if(!playerAlreadyCompletedQuest)
                    {
                        bool playerHasAllItemsToCompleteQuest = true;

                        foreach(QuestCompletionItem qci in newLocation.QuestAvailableHere.QuestCompletionItems)
                        {
                            bool foundItemInPlayerInventory = false; 

                            foreach(InventoryItem ii in _player.Inventory)
                            {
                                if(ii.Details.ID == qci.Details.ID)
                                {
                                    foundItemInPlayerInventory = true; 

                                    if(ii.Quantity < qci.Quantity)
                                    {
                                        playerHasAllItemsToCompleteQuest = false;

                                        break;
                                    }

                                    break;
                                }
                            }


                            if (!foundItemInPlayerInventory)
                            {
                                playerHasAllItemsToCompleteQuest = false;

                                break;
                            }
                        }

                        if (playerHasAllItemsToCompleteQuest)
                        {
                            rtbMessages.Text += Environment.NewLine;
                            rtbMessages.Text += "You complete the " + newLocation.QuestAvailableHere.Name + " quest." + Environment.NewLine;
                              
                            foreach(QuestCompletionItem qci in newLocation.QuestAvailableHere.QuestCompletionItems)
                            {
                                foreach(InventoryItem ii in _player.Inventory)
                                {
                                    if(ii.Details.ID == qci.Details.ID)
                                    {
                                        ii.Quantity -= qci.Quantity;
                                        break;
                                    }
                                }
                            }

                            rtbMessages.Text += "You receive: " + Environment.NewLine;
                            rtbMessages.Text += newLocation.QuestAvailableHere.RewardExperiencePoints.ToString() + " experience points" + Environment.NewLine;
                            rtbMessages.Text += newLocation.QuestAvailableHere.RewardGold.ToString() + " gold" + Environment.NewLine;
                            rtbMessages.Text += newLocation.QuestAvailableHere.RewardItem.Name + Environment.NewLine;
                            rtbMessages.Text += Environment.NewLine;

                            _player.ExperiencePoints += newLocation.QuestAvailableHere.RewardExperiencePoints;
                            _player.Gold += newLocation.QuestAvailableHere.RewardGold;

                            bool addedItemToPlayerInventory = false;

                            foreach (InventoryItem ii in _player.Inventory)
                            {
                                if (ii.Details.ID == newLocation.QuestAvailableHere.RewardItem.ID)
                                {
                                    ii.Quantity++;
                                    addedItemToPlayerInventory = true;
                                    break;
                                }
                            }


                            if (!addedItemToPlayerInventory)
                            {
                                _player.Inventory.Add(new InventoryItem(newLocation.QuestAvailableHere.RewardItem, 1));
                            }

                            foreach (PlayerQuest pq in _player.Quests)
                            {
                                if (pq.Details.ID == newLocation.QuestAvailableHere.ID)
                                {
                                    pq.IsCompleted = true;
                                    break;
                                }
                            }


                        }
                    }
                }
                else
                {
                    rtbMessages.Text += "You receive the " + newLocation.QuestAvailableHere.Name + " quest." + Environment.NewLine;
                    rtbMessages.Text += newLocation.QuestAvailableHere.Description + Environment.NewLine;
                    rtbMessages.Text += "To complete it, return with:" + Environment.NewLine;

                    foreach (QuestCompletionItem qci in newLocation.QuestAvailableHere.QuestCompletionItems)
                    {
                        if (qci.Quantity == 1)
                        {
                            rtbMessages.Text += qci.Quantity.ToString() + " " + qci.Details.Name + Environment.NewLine;
                        } else
                        {
                            rtbMessages.Text += qci.Quantity.ToString() + " " + qci.Details.NamePlural + Environment.NewLine;
                        }
                    }

                    rtbMessages.Text += Environment.NewLine;
                    _player.Quests.Add(new PlayerQuest(newLocation.QuestAvailableHere));

                }
            }

            if (newLocation.MonsterLivingHere != null)
            {
                rtbMessages.Text += "You see a " + newLocation.MonsterLivingHere.Name + Environment.NewLine;

                Monster standardMonster = World.MonsterByID(newLocation.MonsterLivingHere.ID);
                _currentMonster = new Monster(standardMonster.ID, standardMonster.Name, 
                    standardMonster.MaximumDamage, standardMonster.RewardExperiencePoints, standardMonster.RewardGold, standardMonster.CurrentHitPoints, standardMonster.MaximumHitPoints);

                foreach (LootItem lootItem in standardMonster.LootTable)
                {
                    _currentMonster.LootTable.Add(lootItem);
                }

                cboWeapons.Visible = true;
                cboPotions.Visible = true;
                btnUseWeapon.Visible = true;
                btnUsePotion.Visible = true;

            } else
            {
                _currentMonster = null;

                cboWeapons.Visible = false;
                cboPotions.Visible = false;
                btnUseWeapon.Visible = false;
                btnUsePotion.Visible = false;
            }

            dgvInventory.RowHeadersVisible = false;
            dgvInventory.ColumnCount = 2;
            dgvInventory.Columns[0].Name = "Name";
            dgvInventory.Columns[0].Width = 197;
            dgvInventory.Columns[1].Name = "Quantity";

            dgvInventory.Rows.Clear();

            foreach (InventoryItem inventoryItem in _player.Inventory)
            {
                if (inventoryItem.Quantity > 0)
                {
                    dgvInventory.Rows.Add(new[] { inventoryItem.Details.Name, inventoryItem.Quantity.ToString() });
                }
            }


            dgvQuests.RowHeadersVisible = false;
            dgvQuests.ColumnCount = 2;
            dgvQuests.Columns[0].Name = "Name";
            dgvQuests.Columns[0].Width = 197;
            dgvQuests.Columns[1].Name = "Done?"; 
                
            dgvQuests.Rows.Clear();

            foreach (PlayerQuest playerQuest in _player.Quests)
            {
                dgvQuests.Rows.Add(new[] { playerQuest.Details.Name, playerQuest.IsCompleted.ToString() });
            }

            List<Weapon> weapons = new List<Weapon>();

            foreach (InventoryItem inventoryItem in _player.Inventory)
            {
                if (inventoryItem.Details is Weapon)
                {
                    if (inventoryItem.Quantity > 0)
                    {
                        weapons.Add((Weapon)inventoryItem.Details);
                    }
                }
            }

            if (weapons.Count == 0)
            {
                cboWeapons.Visible = false;
                btnUseWeapon.Visible = false;
            }
            else
            {
                cboWeapons.DataSource = weapons;
                cboWeapons.DisplayMember = "Name";
                cboWeapons.ValueMember = "ID";
                cboWeapons.SelectedIndex = 0;
            }

            List<HealingPotion> healingPotions = new List<HealingPotion>();

            foreach (InventoryItem inventoryItem in _player.Inventory)
            {
                if (inventoryItem.Details is HealingPotion)
                {
                    if (inventoryItem.Quantity > 0)
                    {
                        healingPotions.Add((HealingPotion)inventoryItem.Details);
                    }
                }
            }

            if (healingPotions.Count == 0)
            {
                cboPotions.Visible = false;
                btnUsePotion.Visible = false;
            }
            else
            {
                cboPotions.DataSource = healingPotions;
                cboPotions.DisplayMember = "Name";
                cboPotions.ValueMember = "ID";
                cboPotions.SelectedIndex = 0;
            }
        }
        private void BtnUseWeapon_Click(object sender, EventArgs e)
        {

        }

        private void BtnUsePotion_Click(object sender, EventArgs e)
        {

        }
    }
}
