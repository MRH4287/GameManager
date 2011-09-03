using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Game.Game;
using Mysql;

using MySql.Data.MySqlClient;
using System.Reflection;
using System.IO;
using System.Resources;

using GraphicLibary;
using Game.Game.Interface;
using GraphicLibary.Controls;

namespace Game
{
    public partial class Form1 : Form
    {

        // Kompontenten:
        bool connected = false;
        string ip = "127.0.0.1";

        Mysql.MysqlConnection mysql;
        

        GameData data;


        bool changedValue = false;
        bool addEntry = false;

        #region System

        Data.Config config = new Data.Config("Config/config.xml");

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                checkConfig();
                ip = config["server/host"];
                mysql = new Mysql.MysqlConnection();
                mysql.OnError += new EventHandler<Mysql.MysqlConnection.MysqlErrorEventArgs>(mysql_OnError);
                mysql.connect(ip, config["server/database"], config["server/username"], config["server/password"]);

                data = new GameData(mysql, config["server/prefix"]);

                Query.Prefix = config["server/prefix"];

                connected = true;
                ImageSetup();
                InitiateGui();

#if DEBUG
                this.Text += " (Debug-Modus)";
#endif

            }
            catch (Exception ex)
            {

                MessageBox.Show("Es konnte keine Verbindung zu dem Mysql Server aufgebaut werden. Die Funktionalität ist stark eingeschränkt!" + Environment.NewLine +
                   ex.Message);
            }


        }



        private void checkConfig()
        {

            string curFile = @"Config\config.xml";
            if (!File.Exists(curFile))
            {
                MessageBox.Show("Config Datei nicht gefunden!", "Keine Config Datei gefunden", MessageBoxButtons.OK);
                this.Close();

            }
            else
            {
                config.parse(curFile);

            }


        }


        #endregion

        #region GraphhicHelper

        Help help = new Help();

        System.Collections.Hashtable tabl = new System.Collections.Hashtable();


        private void ImageSetup()
        {
            tabl.Add("ok", global::Game.Properties.Resources.Im_ok);
            tabl.Add("warning", global::Game.Properties.Resources.Im_warning);
            tabl.Add("abort", global::Game.Properties.Resources.Im_abort);
            tabl.Add("edit", global::Game.Properties.Resources.Im_edit);
            tabl.Add("loading", global::Game.Properties.Resources.Im_loading);
            tabl.Add("square", global::Game.Properties.Resources.Im_square);

        }

        /// <summary>
        /// Liefert ein bestimmtes Bild zurück
        /// </summary>
        /// <param name="name">Dateiname</param>
        /// <returns>BildDatei</returns>
        public Image getImage(string name)
        {
            return (Image)(tabl[name]);


        }




        #endregion

        #region TabControl Events

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (changedValue || addEntry)
            {
                MessageBox.Show("Warnung es wurden Werte verändert, die vielleicht überschreieben werden könnten. Sie sollten diese Werte speichern");

            }
        }

        private void tabControl2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (changedValue || addEntry)
            {
                MessageBox.Show("Warnung es wurden Werte verändert, die vielleicht überschreieben werden könnten. Sie sollten diese Werte speichern");

            }
        }


        #endregion

        #region GUI

        #region IO

        private void Form1_Resize(object sender, EventArgs e)
        {
            // Vergrößern der Box für die Unterseiten




            //Subpage7: VErgrößern der Anzeige der Forschungen



        }

        delegate void RefreshF(object sender, EventArgs e);



        private void page2_refresh()
        {
            RefreshF d = new RefreshF(page2_select_SelectedIndexChanged);
            this.Invoke(d, new object[] { null, null });

        }

        private void page3_refresh()
        {
            RefreshF d = new RefreshF(page3_select_SelectedIndexChanged);
            this.Invoke(d, new object[] { null, null });

        }




        private void hilfeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {

                help.Show();
            }
            catch
            {
                help = new Help();
                help.Show();
            }
        }


        private void beendenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }



        private void allesAktualisierenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            changedValue = false;
            data.refreshPlanetClassList();
            data.refreshRaceList();
            data.refreshShipClassList();
            data.refreshSkillList();
            data.refreshStationClassList();
            data.refreshTechList();
            data.refreshUserList();

            data.refreshUpdateList();
            data.updateData();

            InitiateGui();



        }


        private void zeitumrechnerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (new TimeCalc()).Show();
        }




        #endregion

        #region Style



        SolarSystemViewer solarviewer = new SolarSystemViewer();


        #endregion

        string keinUpdate = "kein Update";
        string nichtsAusgewählt = "kein Eintrag gewählt";

        private void InitiateGui()
        {


            // Style



            //Initierung der Einträge in der Liste




            //Page2:
            page2_select.Items.Clear();

            foreach (ShipClass ship in data.getShipTypes())
            {
                page2_select.Items.Add(ship);
            }

            //Rassen in Page2:
            page2_race.Items.Clear();

            foreach (Race race in data.getRaces())
            {
                page2_race.Items.Add(race);

            }

            //Technologie Bedinungen
            page2_needs.Items.Clear();

            foreach (Tech tech in data.getTechs())
            {
                page2_needs.Items.Add(tech);
            }

            //Skills
            page2_skills.Items.Clear();

            foreach (Skill skill in data.getSkills())
            {
                if (skill.ship)
                    page2_skills.Items.Add(skill);
            }


            //Page3:

            //Update:

            page3_select.Items.Clear();
            page3_update.Items.Clear();
            page3_update.Items.Add(keinUpdate);
            page3_needs_add_station.Items.Clear();

            foreach (StationClass station in data.getStationTypes())
            {
                page3_select.Items.Add(station);
                page3_update.Items.Add(station);
                page3_needs_add_station.Items.Add(station);
            }

            //Rassen
            page3_race.Items.Clear();

            foreach (Race race in data.getRaces())
            {
                page3_race.Items.Add(race);

            }

            //Technologie Bedinungen
            page3_need_tech.Items.Clear();

            foreach (Tech tech in data.getTechs())
            {
                page3_need_tech.Items.Add(tech);
            }

            //Skills
            page3_skills.Items.Clear();

            foreach (Skill skill in data.getSkills())
            {

                if (skill.stat)
                    page3_skills.Items.Add(skill);
            }


            //Ships:
            page3_buildship.Items.Clear();

            foreach (ShipClass ship in data.getShipTypes())
            {
                page3_buildship.Items.Add(ship);
            }






            //Page5:

            page5_select.Items.Clear();

            foreach (Skill skill in data.getSkills())
            {
                page5_select.Items.Add(skill);
            }





            //Page6:

            page6_select.Items.Clear();

            foreach (Tech tech in data.getTechs())
            {
                page6_select.Items.Add(tech);
            }

            //Rassen
            page6_race.Items.Clear();

            foreach (Race race in data.getRaces())
            {
                page6_race.Items.Add(race);

            }

            //Technologie Bedinungen
            page6_need.Items.Clear();

            foreach (Tech tech in data.getTechs())
            {
                page6_need.Items.Add(tech);
            }

            //Updates:
            page6_updates.Items.Clear();

            foreach (Update update in data.getUpdates())
            {
                page6_updates.Items.Add(update);
            }



            //Page7:



            page7_select.Items.Clear();
            foreach (Update update in data.getUpdates())
            {
                page7_select.Items.Add(update);

            }



            page7_stattype.Items.Clear();
            page7_stattype.Items.Add(nichtsAusgewählt);
            foreach (StationClass station in data.getStationTypes())
            {
                page7_stattype.Items.Add(station);
            }




            //Skills
            page7_skills.Items.Clear();

            foreach (Skill skill in data.getSkills())
            {
                page7_skills.Items.Add(skill);
            }


            //Ships:
            page7_shiptype.Items.Clear();
            page7_shiptype.Items.Add(nichtsAusgewählt);

            foreach (ShipClass ship in data.getShipTypes())
            {
                page7_shiptype.Items.Add(ship);
            }





            //Page8

            page8_select.Items.Clear();
            foreach (PlanetClass planet in data.getPlanetTypes())
            {
                page8_select.Items.Add(planet);

            }

            //Ships:
            page8_buildship.Items.Clear();

            foreach (ShipClass ship in data.getShipTypes())
            {
                page8_buildship.Items.Add(ship);
            }




            page9_select.Items.Clear();

            foreach (Race race in data.getRaces())
            {
                page9_select.Items.Add(race);
            }



            generateResList(page2_reslist);
            generateResList(page3_costs);
            generateResList(page3_res_create);
            generateResList(page6_reslist);
            generateResList(page7_res_reduce);
            generateResList(page8_reslist);
        }



        private double getResourceFromPage(TextboxList holder, ResType type)
        {
            try
            {
                return double.Parse(holder[type.ToString()]);

            }
            catch
            { }
            return 0;
        }


        private void setResourceOnPage(TextboxList holder, ResType type, double value)
        {
            holder[type.ToString()] = value.ToString();
        }



        private void generateResList(TextboxList holder)
        {
            try
            {
                holder.Clear();

                holder.PanelHeight = 30;

                    foreach (ResType type in Enum.GetValues(typeof(ResType)))
                    {
                        holder.AddTextbox(type.ToString());
                    }

            }
            catch
            { }
        }

        private void setResourceOnPage(ResList list, TextboxList holder)
        {

            foreach (KeyValuePair<ResType, double> ent in list)
            {
                setResourceOnPage(holder, ent.Key, ent.Value);
            }

        }

        private ResList getResourceListFromPanel(TextboxList holder)
        {
            ResList list = new ResList();

            foreach (ResType type in Enum.GetValues(typeof(ResType)))
            {
                double value = getResourceFromPage(holder, type);
                list[type] = value;

            }

            return list;
        }

        private void clearResourceList(TextboxList holder)
        {
            foreach (ResType type in Enum.GetValues(typeof(ResType)))
            {
                setResourceOnPage(holder, type, 0);

            }
        }


        #endregion






        #region Page2



        private void page2_textchange(object sender, EventArgs e)
        {
            page2_status.Image = getImage("warning");
            changedValue = true;
        }


        private void page2_select_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShipClass selected = (ShipClass)page2_select.SelectedItem;

            if (selected == null) return;

            page2_name.Text = selected.Name;


            page2_race.Items.Clear();
            foreach (Race race in data.getRaces())
            {
                page2_race.Items.Add(race);
            }


            foreach (Race race in selected.race)
            {

                page2_race.SetItemChecked(page2_race.Items.IndexOf(race), true);
            }


            page2_skills.Items.Clear();
            foreach (Skill skills in data.getSkills())
            {
                if (skills.ship)
                    page2_skills.Items.Add(skills);
            }

            foreach (Skill skills in selected.skills)
            {

                page2_skills.SetItemChecked(page2_skills.Items.IndexOf(skills), true);
            }


            page2_needs.Items.Clear();
            foreach (Tech tech in data.getTechs())
            {
                page2_needs.Items.Add(tech);
            }

            foreach (Tech need in selected.need_tech)
            {

                page2_needs.SetItemChecked(page2_needs.Items.IndexOf(need), true);
            }



            page2_time.Text = selected.time.ToString();
            page2_limit.Text = selected.globallimit.ToString();
            page2_speed.Text = selected.speed.ToString();


            setResourceOnPage(selected.price, page2_reslist);


            page2_power.Text = selected.power.ToString();
            page2_power2.Text = selected.power2.ToString();
            page2_power3.Text = selected.power3.ToString();
            page2_power4.Text = selected.power4.ToString();

            page2_resistend1.Text = selected.resistend1.ToString();
            page2_resistend2.Text = selected.resistend2.ToString();
            page2_resistend3.Text = selected.resistend3.ToString();
            page2_resistend4.Text = selected.resistend4.ToString();

            page2_health.Text = selected.health.ToString();

            //Names:

            page2_namelist.Clear();

            // Bild:

            GraphicHelper graphic = new GraphicHelper(page2_picture);

            graphic.drawRescaleImage(selected.picture, page2_picture.Width, 0, 0);
            page2_picture.Image = graphic.flush();

            

            String[] names = selected.names.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);

            page2_namelist.setContent(names);

            changedValue = false;
            page2_status.Image = getImage("square");
            addEntry = false;

        }

        private void page2_names_add_Click(object sender, EventArgs e)
        {
            if (page2_names_input.Text == "")
            {
                MessageBox.Show("Bitte geben Sie etwas in das Textfeld ein!");
                return;
            }

            if (page2_namelist.Contains(page2_names_input.Text))
            {
                MessageBox.Show("Name bereits enthalten");
                return;
            }

            page2_namelist.Add(page2_names_input.Text);
            page2_names_input.Clear();


            page2_status.Image = getImage("warning");
            changedValue = true;

        }

       
        private void page2_time_edit_Click(object sender, EventArgs e)
        {

            (new TimeCalc(page2_time)).Show();


        }

        private void page2_submit_Click(object sender, EventArgs e)
        {
            if (connected)
            {
                if ((page2_select.SelectedItem == null) && (!addEntry)) { return; }

                string races = "";
                foreach (Race race in page2_race.CheckedItems)
                {
                    if (races != "") { races += ", "; }
                    races += race.id.ToString();
                }

                string skills = "";
                foreach (Skill skill in page2_skills.CheckedItems)
                {
                    if (skills != "") { skills += ", "; }
                    skills += skill.Id.ToString();
                }

                string techs = "";
                foreach (Tech tech in page2_needs.CheckedItems)
                {
                    if (techs != "") { techs += ", "; }
                    techs += tech.Id.ToString();
                }

                string names = "";
                foreach (GraphicLibary.Controls.StringList.StringListContainer text in page2_namelist.getContent())
                {
                    if (names != "") { names += ", "; }
                    names += text.Name;

                }


                Dictionary<string, string> data = new Dictionary<string, string>();

                data.Add("Name", page2_name.Text);


                data.Add("health", page2_health.Text);
                data.Add("Power", page2_power.Text);
                data.Add("Power2", page2_power2.Text);
                data.Add("Power3", page2_power3.Text);
                data.Add("Power4", page2_power4.Text);
                data.Add("Resistend1", page2_resistend1.Text);
                data.Add("Resistend2", page2_resistend2.Text);
                data.Add("Resistend3", page2_resistend3.Text);
                data.Add("Resistend4", page2_resistend4.Text);

                ResList list = getResourceListFromPanel(page2_reslist);
                list.addIntoHashTable(data);


                data.Add("names", names);
                data.Add("time", page2_time.Text);
                data.Add("race", races);
                data.Add("speed", page2_speed.Text);
                data.Add("skills", page2_skills.Text);
                data.Add("globallimit", page2_limit.Text);
                data.Add("need_tech", techs);

                if (page2_picture.Image != null)
                {
                    Bitmap bitmap = new Bitmap(page2_picture.Image);
                    MemoryStream stream = new MemoryStream();
                    bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);

                    data.Add("picture", stream.GetBuffer().ToString());
                }

                if (!addEntry)
                {
                    Query qr = Query.Update("ships").Where("ID", ((ShipClass)page2_select.SelectedItem).Id.ToString());
                    qr.Set(data);
                    mysql.Query(qr);
                }
                else
                {
                    addEntry = false;

                    Query.Insert("ships").Set(data).execute(mysql);
                }

                this.data.refreshShipClassList();
                this.data.updateData();


                changedValue = false;


                InitiateGui();

                page2_refresh();
                page2_status.Image = getImage("ok");


            }
            else
            {
                MessageBox.Show("keine Mysql Verbindung");
            }
        }

        private void page2_addEntry_Click(object sender, EventArgs e)
        {
            if (changedValue)
            {
                MessageBox.Show("Nicht möglich: ungespeicherte Elemente!");
            }
            else
            {

                InitiateGui();
                page2_name.Clear();
                page2_limit.Clear();
                page2_speed.Clear();
                page2_time.Clear();
                clearResourceList(page2_reslist);
                page2_picture.Image = (new GraphicHelper(1, 1)).flush();
                page2_power.Clear();
                page2_power2.Clear();
                page2_power3.Clear();
                page2_power4.Clear();
                page2_resistend1.Clear();
                page2_resistend2.Clear();
                page2_resistend3.Clear();
                page2_resistend4.Clear();
                page2_health.Clear();

                page2_select.Text = "New Entry";

                page2_status.Image = getImage("edit");
                //Names:

                page2_namelist.Clear();



                addEntry = true;

            }
        }

        private void page2_delete_Click(object sender, EventArgs e)
        {
            if (!addEntry)
            {
                ShipClass item = (ShipClass)page2_select.SelectedItem;
                if (item != null)
                {
                    System.Windows.Forms.DialogResult result = MessageBox.Show("Möchten Sie den Schiffstyp (ID: " + item.Id.ToString() + ") wirklich löschen?", "Löschen", MessageBoxButtons.YesNo);

                    if (result == System.Windows.Forms.DialogResult.Yes)
                    {
                        data.Query("DELETE FROM `PX_ships` WHERE `ID` = '" + item.Id.ToString() + "';").Close();
                        page2_status.Image = getImage("abort");

                        data.refreshShipClassList();

                    }
                }
            }
            else
            {
                MessageBox.Show("Nicht möglich!");
            }

        }




        #endregion

        #region Page3

       

        System.Collections.Hashtable page3_needs_EntryList = new System.Collections.Hashtable();






        private void page3_textchange(object sender, EventArgs e)
        {
            page3_status.Image = getImage("warning");
            changedValue = true;
        }

        private void page3_select_SelectedIndexChanged(object sender, EventArgs e)
        {
            StationClass selected = (StationClass)page3_select.SelectedItem;

            if (selected == null) return;

            page3_name.Text = selected.Name;


            page3_race.Items.Clear();
            foreach (Race race in data.getRaces())
            {
                page3_race.Items.Add(race);
            }


            foreach (Race race in selected.race)
            {

                page3_race.SetItemChecked(page3_race.Items.IndexOf(race), true);
            }


            page3_skills.Items.Clear();
            foreach (Skill skills in data.getSkills())
            {
                if (skills.stat)
                    page3_skills.Items.Add(skills);
            }

            foreach (Skill skills in selected.skills)
            {

                page3_skills.SetItemChecked(page3_skills.Items.IndexOf(skills), true);
            }


            page3_need_tech.Items.Clear();
            foreach (Tech tech in data.getTechs())
            {
                page3_need_tech.Items.Add(tech);
            }

            foreach (Tech need in selected.need_tech)
            {

                page3_need_tech.SetItemChecked(page3_need_tech.Items.IndexOf(need), true);
            }


            page3_buildship.Items.Clear();
            foreach (ShipClass ship in data.getShipTypes())
            {
                page3_buildship.Items.Add(ship);
            }

            foreach (ShipClass ship in selected.buildship)
            {

                page3_buildship.SetItemChecked(page3_buildship.Items.IndexOf(ship), true);
            }


            page3_time.Text = selected.time.ToString();
            page3_limit.Text = selected.limit.ToString();
            page3_globallimit.Text = selected.globallimit.ToString();

            if (selected.updateto != null)
            {
                page3_update.SelectedItem = selected.updateto;
            }
            else
            {
                page3_update.SelectedItem = keinUpdate;
            }

            page3_population.Text = selected.population.ToString();


            
            setResourceOnPage(selected.create_res, page3_res_create);
            setResourceOnPage(selected.price, page3_costs);
            
            page3_power.Text = selected.power.ToString();
            page3_power2.Text = selected.power2.ToString();
            page3_power3.Text = selected.power3.ToString();
            page3_power4.Text = selected.power4.ToString();

            page3_resistend1.Text = selected.resistend1.ToString();
            page3_resistend2.Text = selected.resistend2.ToString();
            page3_resistend3.Text = selected.resistend3.ToString();
            page3_resistend4.Text = selected.resistend4.ToString();

            page3_health.Text = selected.health.ToString();

            page3_names.Clear();
            page3_needlist.Clear();


            // Bild:

            GraphicHelper graphic = new GraphicHelper(page3_picture);
            graphic.drawCenterImage(selected.picture, page3_picture.Width);
            page3_picture.Image = graphic.flush();


            


            String[] names = selected.names.Split(new string[] { ", " }, StringSplitOptions.None);

            page3_names.setContent(names);



            page3_needlist.OnStringListClick +=new EventHandler<StringList.StringListClickEvent>(page3_needs_delete);


            page3_needs_EntryList = selected.need;

            foreach (System.Collections.DictionaryEntry entry in selected.need)
            {
                page3_needlist.Add(((int)entry.Value).ToString() + "X " + ((StationClass)entry.Key).Name, entry.Key);
          

            }


            addEntry = false;
            changedValue = false;
            page3_status.Image = getImage("square");
        }


        private void page3_time_edit_Click(object sender, EventArgs e)
        {
            (new TimeCalc(page3_time)).Show();
        }

        private void page3_submit_Click(object sender, EventArgs e)
        {
            if (connected)
            {
                if ((page3_select.SelectedItem == null) && (!addEntry)) { return; }

                string races = "";
                foreach (Race race in page3_race.CheckedItems)
                {
                    if (races != "") { races += ", "; }
                    races += race.id.ToString();
                }

                string skills = "";
                foreach (Skill skill in page3_skills.CheckedItems)
                {
                    if (skills != "") { skills += ", "; }
                    skills += skill.Id.ToString();
                }

                string techs = "";
                foreach (Tech tech in page3_need_tech.CheckedItems)
                {
                    if (techs != "") { techs += ", "; }
                    techs += tech.Id.ToString();
                }


                string buildships = "";
                foreach (ShipClass ship in page3_buildship.CheckedItems)
                {
                    if (buildships != "") { buildships += ", "; }
                    buildships += ship.Id.ToString();
                }


                string names = "";
                foreach (GraphicLibary.Controls.StringList.StringListContainer text in page3_names.getContent())
                {
                    if (names != "") { names += ", "; }
                    names += text.Name;

                }

                


                string needs = "";
                string needcount = "";

                foreach (System.Collections.DictionaryEntry entry in page3_needs_EntryList)
                {
                    if (needs != "") { needs += ", "; }
                    if (needcount != "") { needcount += ", "; }

                    needs += ((StationClass)entry.Key).Id.ToString(); ;
                    needcount += ((int)entry.Value).ToString();


                }

                ResList price = getResourceListFromPanel(page3_res_create);
                string[] tmp = Game.Static.ResHelper.getResText(price);
                string buildres = tmp[0];
                string res = tmp[1];



                string updateto = "0";
                if (!(page3_update.SelectedItem is String))
                {
                    updateto = ((StationClass)page3_update.SelectedItem).Id.ToString();

                }




                Dictionary<string, string> data = new Dictionary<string, string>();

                data.Add("Name", page3_name.Text);


                data.Add("health", page3_health.Text);
                data.Add("Power", page3_power.Text);
                data.Add("Power2", page3_power2.Text);
                data.Add("Power3", page3_power3.Text);
                data.Add("Power4", page3_power4.Text);
                data.Add("Resistend1", page3_resistend1.Text);
                data.Add("Resistend2", page3_resistend2.Text);
                data.Add("Resistend3", page3_resistend3.Text);
                data.Add("Resistend4", page3_resistend4.Text);

                ResList list = getResourceListFromPanel(page3_costs);
                list.addIntoHashTable(data);
                


                data.Add("names", names);
                data.Add("time", page3_time.Text);
                data.Add("race", races);

                data.Add("skills", page3_skills.Text);
                data.Add("globallimit", page3_limit.Text);
                data.Add("need_tech", techs);

                data.Add("buildship", buildships);

                
                
                data.Add("buildres", buildres);
                data.Add("res", res);
                
                 

                data.Add("updateto", updateto);
                data.Add("limit", page3_limit.Text);
                data.Add("need", needs);
                data.Add("needcount", needcount);
                data.Add("population", page3_population.Text);

                if (!addEntry)
                {
                    Query.Update("stations").Set(data).Where("ID", ((StationClass)page3_select.SelectedItem).Id.ToString()).execute(mysql);
                }
                else
                {
                    addEntry = false;
                    Query.Insert("stations").Set(data).execute(mysql);

                }

                this.data.refreshStationClassList();
                this.data.updateData();


                changedValue = false;

                InitiateGui();

                page3_refresh();
                page3_status.Image = getImage("ok");


            }
            else
            {
                MessageBox.Show("keine Mysql Verbindung");
            }
        }


        private void page3_needs_delete(object sender, StringList.StringListClickEvent e)
        {

            try
            {

                StationClass klasse = (StationClass)e.Hold;


                page3_needs_EntryList.Remove(klasse);

                changedValue = true;
                page3_status.Image = getImage("warning");


            }
            catch
            {

            }


        }


        private void page3_names_add_Click(object sender, EventArgs e)
        {
            if (page3_names_input.Text == "")
            {
                MessageBox.Show("Bitte geben Sie etwas in das Textfeld ein!");
                return;
            }

            if (page2_namelist.Contains(page3_names_input.Text))
            {
                MessageBox.Show("Name bereits enthalten");
                return;
            }

            page3_names.Add(page3_names_input.Text);
            page3_names_input.Clear();


            page3_status.Image = getImage("warning");
            changedValue = true;

        }

        private void page3_needs_add_Click(object sender, EventArgs e)
        {
            if ((page3_needs_add_count.Text != "") && (page3_needs_add_station.SelectedItem != null))
            {
                try
                {
                  

                    StationClass add = (StationClass)page3_needs_add_station.SelectedItem;
                    int count = int.Parse(page3_needs_add_count.Text);

                    string text = count.ToString() + "X " + add.Name;

                    page3_needs_EntryList.Add(add, count);

                    page3_needlist.Add(text, add);


                    page3_needs_add_count.Clear();


                    


                    page3_status.Image = getImage("warning");
                    changedValue = true;

                }
                catch
                {
                    MessageBox.Show("Fehlerhafte Eingabe");
                }

            }
            else
            {
                MessageBox.Show("Bitte geben Sie etwas ein");
            }
        }

        private void page3_addEntry_Click(object sender, EventArgs e)
        {
            if (changedValue)
            {
                MessageBox.Show("Nicht möglich: ungespeicherte Elemente!");
            }
            else
            {

                InitiateGui();
                page3_name.Clear();
                page3_limit.Clear();
                page3_time.Clear();

                clearResourceList(page3_costs);
                clearResourceList(page3_res_create);

                page3_picture.Image = (new GraphicHelper(1, 1)).flush();
                page3_power.Clear();
                page3_power2.Clear();
                page3_power3.Clear();
                page3_power4.Clear();
                page3_resistend1.Clear();
                page3_resistend2.Clear();
                page3_resistend3.Clear();
                page3_resistend4.Clear();
                page3_health.Clear();

                page3_globallimit.Clear();
                page3_update.SelectedItem = keinUpdate;
                page3_population.Clear();


                page3_select.Text = "New Entry";
                page3_status.Image = getImage("edit");


                page3_names.Clear();
                page3_needlist.Clear();






                addEntry = true;

            }
        }

        private void page3_delete_Click(object sender, EventArgs e)
        {
            if (!addEntry)
            {
                StationClass item = (StationClass)page3_select.SelectedItem;
                if (item != null)
                {
                    System.Windows.Forms.DialogResult result = MessageBox.Show("Möchten Sie den Stations Typ (ID: " + item.Id.ToString() + ") wirklich löschen?", "Löschen", MessageBoxButtons.YesNo);

                    if (result == System.Windows.Forms.DialogResult.Yes)
                    {
                        data.Query("DELETE FROM `PX_stations` WHERE `ID` = '" + item.Id.ToString() + "';").Close();
                        page3_status.Image = getImage("abort");

                        data.refreshStationClassList();

                    }
                }
            }
            else
            {
                MessageBox.Show("Nicht möglich!");
            }
        }


        #endregion


        #region Page5

        private void page5_time_edit_Click(object sender, EventArgs e)
        {
            (new TimeCalc(page5_time)).Show();
        }

        private void page5_select_SelectedIndexChanged(object sender, EventArgs e)
        {
            Skill selected = (Skill)page5_select.SelectedItem;

            if (selected == null) return;

            page5_name.Text = selected.Name;



            page5_time.Text = selected.time.ToString();

            page5_beschreibung.Text = selected.Beschreibung;
            page5_states.Text = selected.states;

            page5_passiv.Checked = selected.passiv;
            page5_ship.Checked = selected.ship;
            page5_stat.Checked = selected.stat;
            page5_skill.Text = selected.skill;



            addEntry = false;
            changedValue = false;
            page5_status.Image = getImage("square");
        }

        private void page5_textchange(object sender, EventArgs e)
        {
            page5_status.Image = getImage("warning");
            changedValue = true;
        }

        private void page5_submit_Click(object sender, EventArgs e)
        {
            if (connected)
            {
                if ((page5_select.SelectedItem == null) && (!addEntry)) { return; }



                Dictionary<string, string> data = new Dictionary<string, string>();

                data.Add("Name", page5_name.Text);
                data.Add("skill", page5_skill.Text);
                data.Add("beschreibung", page5_beschreibung.Text);
                data.Add("states", page5_states.Text);

                data.Add("passiv", (page5_passiv.Checked) ? "1" : "0");
                data.Add("ship", (page5_ship.Checked) ? "1" : "0");
                data.Add("stat", (page5_stat.Checked) ? "1" : "0");
                data.Add("time", page5_time.Text);



                if (!addEntry)
                {
                    Query.Update("skills").Set(data).Where("ID", ((Skill)page5_select.SelectedItem).Id.ToString()).execute(mysql);
                }
                else
                {
                    addEntry = false;
                    Query.Insert("skills").Set(data).execute(mysql);

                }

                this.data.refreshSkillList();
                this.data.updateData();


                changedValue = false;


                InitiateGui();


                page5_status.Image = getImage("ok");


            }
            else
            {
                MessageBox.Show("keine Mysql Verbindung");
            }
        }

        private void page5_addEntry_Click(object sender, EventArgs e)
        {
            if (changedValue)
            {
                MessageBox.Show("Nicht möglich: ungespeicherte Elemente!");
            }
            else
            {

                InitiateGui();
                page5_name.Clear();
                page5_beschreibung.Clear();
                page5_passiv.Checked = false;
                page5_ship.Checked = false;
                page5_stat.Checked = false;
                page5_skill.Clear();
                page5_states.Clear();

                page5_time.Clear();


                page5_select.Text = "New Entry";

                page5_status.Image = getImage("edit");


                addEntry = true;

            }
        }

        private void page5_delet_Click(object sender, EventArgs e)
        {
            if (!addEntry)
            {
                Skill item = (Skill)page5_select.SelectedItem;
                if (item != null)
                {
                    System.Windows.Forms.DialogResult result = MessageBox.Show("Möchten Sie die Fähigkeit (ID: " + item.Id.ToString() + ") wirklich löschen?", "Löschen", MessageBoxButtons.YesNo);

                    if (result == System.Windows.Forms.DialogResult.Yes)
                    {
                        data.Query("DELETE FROM `PX_skills` WHERE `ID` = '" + item.Id.ToString() + "';").Close();
                        page5_status.Image = getImage("abort");

                        data.refreshSkillList();

                    }
                }
            }
            else
            {
                MessageBox.Show("Nicht möglich!");
            }
        }



        #endregion

        #region Page6

        private void page6_time_edit_Click(object sender, EventArgs e)
        {
            (new TimeCalc(page6_time)).Show();
        }

        private void page6_select_SelectedIndexChanged(object sender, EventArgs e)
        {
            Tech selected = (Tech)page6_select.SelectedItem;

            if (selected == null) return;

            page6_name.Text = selected.Name;


            page6_race.Items.Clear();
            foreach (Race race in data.getRaces())
            {
                page6_race.Items.Add(race);
            }


            foreach (Race race in selected.race)
            {

                page6_race.SetItemChecked(page6_race.Items.IndexOf(race), true);
            }


            page6_updates.Items.Clear();
            foreach (Update updates in data.getUpdates())
            {
                page6_updates.Items.Add(updates);
            }

            foreach (Update updates in selected.update)
            {

                page6_updates.SetItemChecked(page6_updates.Items.IndexOf(updates), true);
            }


            page6_need.Items.Clear();
            foreach (Tech tech in data.getTechs())
            {
                page6_need.Items.Add(tech);
            }

            foreach (Tech need in selected.need_tech)
            {

                page6_need.SetItemChecked(page6_need.Items.IndexOf(need), true);
            }



            page6_time.Text = selected.time.ToString();



            setResourceOnPage(selected.price, page6_reslist);

            page6_group.Text = selected.group.ToString();
            page6_beschreibung.Text = selected.Beschreibung;



            addEntry = false;
            changedValue = false;
            page6_status.Image = getImage("square");
        }

        private void page6_submit_Click(object sender, EventArgs e)
        {
            if (connected)
            {
                if ((page6_select.SelectedItem == null) && (!addEntry)) { return; }

                string races = "";
                foreach (Race race in page6_race.CheckedItems)
                {
                    if (races != "") { races += ", "; }
                    races += race.id.ToString();
                }

                string updates = "";
                foreach (Update update in page6_updates.CheckedItems)
                {
                    if (updates != "") { updates += ", "; }
                    updates += update.Id.ToString();
                }

                string techs = "";
                foreach (Tech tech in page6_need.CheckedItems)
                {
                    if (techs != "") { techs += ", "; }
                    techs += tech.Id.ToString();
                }



                Dictionary<string, string> data = new Dictionary<string, string>();

                data.Add("Name", page6_name.Text);

                ResList list = getResourceListFromPanel(page6_reslist);
                list.addIntoHashTable(data);

                data.Add("time", page6_time.Text);
                data.Add("race", races);
                data.Add("update", updates);
                data.Add("group", page6_group.Text);
                data.Add("need", techs);
                data.Add("beschreibung", page6_beschreibung.Text);


                if (!addEntry)
                {
                    Query.Update("tech").Set(data).Where("ID", ((Tech)page6_select.SelectedItem).Id.ToString()).execute(mysql); 
                 }
                else
                {
                    addEntry = false;

                    Query.Insert("tech").Set(data).execute(mysql);

                }

                this.data.refreshTechList();
                this.data.updateData();


                changedValue = false;


                InitiateGui();

                page6_status.Image = getImage("ok");


            }
            else
            {
                MessageBox.Show("keine Mysql Verbindung");
            }
        }

        private void page6_addEntry_Click(object sender, EventArgs e)
        {
            if (changedValue)
            {
                MessageBox.Show("Nicht möglich: ungespeicherte Elemente!");
            }
            else
            {

                InitiateGui();
                page6_name.Clear();
                page6_beschreibung.Clear();

                clearResourceList(page6_reslist);

                page6_time.Clear();


                page6_select.Text = "New Entry";

                page6_status.Image = getImage("edit");


                addEntry = true;

            }
        }

        private void page6_delet_Click(object sender, EventArgs e)
        {
            if (!addEntry)
            {
                Tech item = (Tech)page6_select.SelectedItem;
                if (item != null)
                {
                    System.Windows.Forms.DialogResult result = MessageBox.Show("Möchten Sie die Technologie (ID: " + item.Id.ToString() + ") wirklich löschen?", "Löschen", MessageBoxButtons.YesNo);

                    if (result == System.Windows.Forms.DialogResult.Yes)
                    {
                        data.Query("DELETE FROM `PX_tech` WHERE `ID` = '" + item.Id.ToString() + "';").Close();
                        page6_status.Image = getImage("abort");

                        data.refreshTechList();

                    }
                }
            }
            else
            {
                MessageBox.Show("Nicht möglich!");
            }
        }


        #endregion

        #region Page7


        private void page7_time_edit_Click(object sender, EventArgs e)
        {
            (new TimeCalc(page7_time)).Show();
        }

        private void page7_select_SelectedIndexChanged(object sender, EventArgs e)
        {
            Update selected = (Update)page7_select.SelectedItem;

            if (selected == null) return;

            page7_name.Text = selected.Name;



            page7_skills.Items.Clear();
            foreach (Skill skills in data.getSkills())
            {
                page7_skills.Items.Add(skills);
            }

            foreach (Skill skills in selected.skills)
            {

                page7_skills.SetItemChecked(page7_skills.Items.IndexOf(skills), true);
            }





            page7_time.Text = selected.time.ToString();
            page7_hide.Text = selected.hide.ToString();

         
            setResourceOnPage(selected.reduceCost, page7_res_reduce);
    

            page7_power.Text = selected.strength.ToString();
            page7_power2.Text = selected.strength2.ToString();
            page7_power3.Text = selected.strength3.ToString();
            page7_power4.Text = selected.strength4.ToString();

            page7_resistend1.Text = selected.resistend1.ToString();
            page7_resistend2.Text = selected.resistend2.ToString();
            page7_resistend3.Text = selected.resistend3.ToString();
            page7_resistend4.Text = selected.resistend4.ToString();

            page7_speed.Text = selected.speed.ToString();

            if (selected.shiptype != null)
            {
                page7_shiptype.SelectedItem = selected.shiptype;
            }
            else
            {
                page7_shiptype.SelectedItem = nichtsAusgewählt;
            }

            if (selected.stattype != null)
            {
                page7_stattype.SelectedItem = selected.stattype;
            }
            else
            {
                page7_stattype.SelectedItem = nichtsAusgewählt;
            }




            addEntry = false;
            changedValue = false;
            page7_status.Image = getImage("square");


        }

        private void page7_submit_Click(object sender, EventArgs e)
        {
            if (connected)
            {
                if ((page7_select.SelectedItem == null) && (!addEntry)) { return; }


                string skills = "";
                foreach (Skill skill in page7_skills.CheckedItems)
                {
                    if (skills != "") { skills += ", "; }
                    skills += skill.Id.ToString();
                }

                string shiptype = "0";
                if (!(page7_shiptype.SelectedItem is String))
                {
                    shiptype = ((ShipClass)page7_shiptype.SelectedItem).Id.ToString();
                }

                string stattype = "0";
                if (!(page7_stattype.SelectedItem is String))
                {
                    stattype = ((StationClass)page7_stattype.SelectedItem).Id.ToString();
                }




                Dictionary<string, string> data = new Dictionary<string, string>();

                data.Add("Name", page7_name.Text);

                data.Add("strength", page7_power.Text);
                data.Add("strength2", page7_power2.Text);
                data.Add("strength3", page7_power3.Text);
                data.Add("strength4", page7_power4.Text);
                data.Add("resistend1", page7_resistend1.Text);
                data.Add("resistend2", page7_resistend2.Text);
                data.Add("resistend3", page7_resistend3.Text);
                data.Add("resistend4", page7_resistend4.Text);

                ResList list = getResourceListFromPanel(page7_res_reduce);
                list.addIntoHashTable(data);


                data.Add("hide", page7_hide.Text);
                data.Add("time", page7_time.Text);
                data.Add("skills", skills);

                data.Add("shiptype", shiptype);
                data.Add("stattype", stattype);


                if (!addEntry)
                {
                    Query.Update("update").Set(data).Where("ID", ((Update)page7_select.SelectedItem).Id.ToString()).execute(mysql);
                }
                else
                {
                    addEntry = false;
                    Query.Insert("update").Set(data).execute(mysql);

                }

                this.data.refreshUpdateList();
                this.data.updateData();


                changedValue = false;


                InitiateGui();

                page7_status.Image = getImage("ok");


            }
            else
            {
                MessageBox.Show("keine Mysql Verbindung");
            }
        }

        private void page7_addEntry_Click(object sender, EventArgs e)
        {
            if (changedValue)
            {
                MessageBox.Show("Nicht möglich: ungespeicherte Elemente!");
            }
            else
            {

                InitiateGui();
                page7_name.Clear();

                page7_hide.Clear();
                page7_time.Clear();

                clearResourceList(page7_res_reduce);
                page7_power.Clear();
                page7_power2.Clear();
                page7_power3.Clear();
                page7_power4.Clear();
                page7_resistend1.Clear();
                page7_resistend2.Clear();
                page7_resistend3.Clear();
                page7_resistend4.Clear();
                page7_shiptype.SelectedItem = nichtsAusgewählt;
                page7_stattype.SelectedItem = nichtsAusgewählt;
                page7_speed.Clear();


                page7_select.Text = "New Entry";

                page7_status.Image = getImage("edit");


                addEntry = true;

            }
        }

        private void page7_delete_Click(object sender, EventArgs e)
        {
            if (!addEntry)
            {
                Update item = (Update)page7_select.SelectedItem;
                if (item != null)
                {
                    System.Windows.Forms.DialogResult result = MessageBox.Show("Möchten Sie das Update (ID: " + item.Id.ToString() + ") wirklich löschen?", "Löschen", MessageBoxButtons.YesNo);

                    if (result == System.Windows.Forms.DialogResult.Yes)
                    {
                        data.Query("DELETE FROM `PX_update` WHERE `ID` = '" + item.Id.ToString() + "';").Close();
                        page6_status.Image = getImage("abort");

                        data.refreshUpdateList();

                    }
                }
            }
            else
            {
                MessageBox.Show("Nicht möglich!");
            }
        }

        #endregion

        #region Page8

        private void page8_select_SelectedIndexChanged(object sender, EventArgs e)
        {
            PlanetClass selected = (PlanetClass)page8_select.SelectedItem;

            if (selected == null) return;

            page8_name.Text = selected.Name;




            page8_buildship.Items.Clear();
            foreach (ShipClass ship in data.getShipTypes())
            {
                page8_buildship.Items.Add(ship);
            }

            foreach (ShipClass ship in selected.buildship)
            {

                page8_buildship.SetItemChecked(page8_buildship.Items.IndexOf(ship), true);
            }

            setResourceOnPage(selected.create_res, page8_reslist);
          

     

            //Names:

            /*
            foreach (Panel panel in page3_names_panelList)
            {
                page3_names_list.Controls.Remove(panel);
                panel.Dispose();

            }
            */


            page8_bewohnbar.Checked = selected.bewohnbar;

            // Bild:

            GraphicHelper graphic = new GraphicHelper(page8_picture);
            graphic.drawCenterImage(selected.picture, page8_picture.Width);
            page8_picture.Image = graphic.flush();



            addEntry = false;
            changedValue = false;
            page8_status.Image = getImage("square");
        }

        private void page8_submit_Click(object sender, EventArgs e)
        {
            if (connected)
            {
                if ((page8_select.SelectedItem == null) && (!addEntry)) { return; }



                string buildships = "";
                foreach (ShipClass ship in page8_buildship.CheckedItems)
                {
                    if (buildships != "") { buildships += ", "; }
                    buildships += ship.Id.ToString();
                }


                ResList list = getResourceListFromPanel(page8_reslist);
                string[] tmp = Game.Static.ResHelper.getResText(list);
                string buildres = tmp[0];
                string res = tmp[1];



                Dictionary<string, string> data = new Dictionary<string, string>();

              
                
                data.Add("Name", page8_name.Text);
                data.Add("buildship", buildships);
                data.Add("buildres", buildres);
                data.Add("res", res);
                
                data.Add("bewohnbar", (page8_bewohnbar.Checked) ? "1" : "0");
                

                if (!addEntry)
                {
                    Query.Update("planeten").Set(data).Where("ID", ((PlanetClass)page8_select.SelectedItem).Id.ToString()).execute(mysql);
                }
                else
                {
                    addEntry = false;
                    Query.Insert("planeten").Set(data).execute(mysql);

                }

                this.data.refreshPlanetClassList();
                this.data.updateData();


                changedValue = false;

                InitiateGui();


                page8_status.Image = getImage("ok");


            }
            else
            {
                MessageBox.Show("keine Mysql Verbindung");
            }
        }

        private void page8_addEntry_Click(object sender, EventArgs e)
        {
            if (changedValue)
            {
                MessageBox.Show("Nicht möglich: ungespeicherte Elemente!");
            }
            else
            {

                InitiateGui();
                page8_name.Clear();

                
                page8_bewohnbar.Checked = false;


                clearResourceList(page8_reslist);

                page8_select.Text = "New Entry";

                page8_status.Image = getImage("edit");


                addEntry = true;

            }
        }

        private void page8_delete_Click(object sender, EventArgs e)
        {
            if (!addEntry)
            {
                PlanetClass item = (PlanetClass)page8_select.SelectedItem;
                if (item != null)
                {
                    System.Windows.Forms.DialogResult result = MessageBox.Show("Möchten Sie die Planeten Klasse (ID: " + item.Id.ToString() + ") wirklich löschen?", "Löschen", MessageBoxButtons.YesNo);

                    if (result == System.Windows.Forms.DialogResult.Yes)
                    {
                        data.Query("DELETE FROM `PX_planeten` WHERE `ID` = '" + item.Id.ToString() + "';").Close();
                        page6_status.Image = getImage("abort");

                        data.refreshUpdateList();

                    }
                }
            }
            else
            {
                MessageBox.Show("Nicht möglich!");
            }
        }



        #endregion

        #region Page9

        private void page9_select_SelectedIndexChanged(object sender, EventArgs e)
        {
            Race selected = (Race)page9_select.SelectedItem;

            if (selected == null) return;

            page9_name.Text = selected.name;


            changedValue = false;
            addEntry = false;

            page9_status.Image = getImage("square");
        }

        private void page9_submit_Click(object sender, EventArgs e)
        {
            if (connected)
            {
                if ((page9_select.SelectedItem == null) && (!addEntry)) { return; }





                Dictionary<string, string> data = new Dictionary<string, string>();

                data.Add("name", page9_name.Text);


                if (!addEntry)
                {
                    Query.Update("race").Set(data).Where("ID", ((Race)page9_select.SelectedItem).id.ToString()).execute(mysql);
                }
                else
                {
                    addEntry = false;
                    Query.Insert("race").Set(data).execute(mysql);

                }

                this.data.refreshRaceList();
                this.data.updateData();


                changedValue = false;

                InitiateGui();


                page9_status.Image = getImage("ok");
            }
        }

        private void page9_addEntry_Click(object sender, EventArgs e)
        {
            if (changedValue)
            {
                MessageBox.Show("Nicht möglich: ungespeicherte Elemente!");
            }
            else
            {

                InitiateGui();
                page9_name.Clear();


                page9_select.Text = "New Entry";

                page9_status.Image = getImage("edit");


                addEntry = true;

            }
        }

        private void page9_delete_Click(object sender, EventArgs e)
        {
            if (!addEntry)
            {
                Race item = (Race)page9_select.SelectedItem;
                if (item != null)
                {
                    System.Windows.Forms.DialogResult result = MessageBox.Show("Möchten Sie die Rasse (ID: " + item.id.ToString() + ") wirklich löschen?", "Löschen", MessageBoxButtons.YesNo);

                    if (result == System.Windows.Forms.DialogResult.Yes)
                    {
                        data.Query("DELETE FROM `PX_race` WHERE `ID` = '" + item.id.ToString() + "';").Close();
                        page6_status.Image = getImage("abort");

                        data.refreshRaceList();

                    }
                }
            }
            else
            {
                MessageBox.Show("Nicht möglich!");
            }
        }



        #endregion




        void mysql_OnError(object sender, Mysql.MysqlConnection.MysqlErrorEventArgs e)
        {
            MessageBox.Show(e.getException().Message);
        }


        private void pictureBox4_Click(object sender, EventArgs e)
        {
            help.k++;
            help.passPhrase = page6_name.Text;
        }

        private void pictureBox4_DoubleClick(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            GameDataGenerator gen = new GameDataGenerator(data);
            gen.Show();
        }

        private void zeitUmrechnerToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            TimeCalc calc = new TimeCalc();
            calc.Show();
        }

        private void ladeAusGameDataDateiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Wenn Sie diese Aktion durchführen, werden alle Änderungen am System, die Sie noch nicht gespeichert haben gelöscht!" + Environment.NewLine +
                 "Dieser Modus dient nur zum Auslesen der Daten! Wenn Sie diese ändern möchten, starten Sie den Mysql-Modus und exportieren diese!" + Environment.NewLine +
             "Möchten Sie den Modus starten?", "Warnung", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "GameData-Datei | GameData.dat";
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    connected = false;
                    data = new GameData(dialog.FileName);

                    Text += " - GameData Mode"; 
                    
                    InitiateGui();

                }

            }

        }

        private void starteMysqlModusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!connected)
            {
                data = new GameData(mysql, config["server/prefix"]);
                connected = true;

                allesAktualisierenToolStripMenuItem_Click(this, new EventArgs());
                InitiateGui();

                string[] title = Text.Split(new string[] { " - " }, StringSplitOptions.None);
                Text = title[0];

                MessageBox.Show("Mysql-Modus gestartet");

            }
            else
            {
                MessageBox.Show("Sie sind berets im Mysql-Modus!");
            }
        }

        private void page3_image_search_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Bilder | *.jpg;*.png;*.bmp";

            if (open.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                
                FileStream stream = File.OpenRead(open.FileName);
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, (int)stream.Length);

                GraphicHelper graphic = new GraphicHelper(page3_picture);
                graphic.drawCenterImage(GraphicHelper.getPicture(buffer), page3_picture.Width);
                page3_picture.Image = graphic.flush();

            }


        }
    }
}
