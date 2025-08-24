using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VMS.TPS.Common.Model.API;

namespace june19
{
    /// <summary>
    /// Interaction logic for UI.xaml
    /// </summary>
    public partial class UI : UserControl
    {
        public List<string> vs1 = new List<string>();
        public List<string> vs2 = new List<string>();
        public List<string> vs3 = new List<string>();
        public List<string> vs4 = new List<string>();
        public StructureSet structureSet { get;set; }

        public UI(ScriptContext context)
        {
            InitializeComponent();            
            Patient patient = context.Patient;
            patient.BeginModifications();
            StructureSet structureSet1 = patient.StructureSets.First();
            structureSet = structureSet1;
            Structure[] newS = structureSet.Structures.ToArray();
            foreach(var z in newS)
            {
                cb.Items.Add(z.Id);
            }
            
            inner_margin.Text = "10.0";
            outer_margin.Text = "30.0";
            ring_name.Text = "ring1";

            foreach(var xx in newS)
            {
                LSTBX.Items.Add(xx.Id);
            }
                
                       
        }
        

        
        private void PtvAdd_Click(object sender, RoutedEventArgs e)
        {
            selected_ptv.Text = LSTBX.SelectedItem.ToString();           

        }

        private void PtvAdd2_Click(object sender, RoutedEventArgs e)
        {
            selected_ptv2.Text = LSTBX.SelectedItem.ToString();
        }

        private void OarAdd_Click(object sender, RoutedEventArgs e)
        {
            abcd.Items.Add(LSTBX.SelectedItem);
        }
        
        private void clear_ptv_Click(object sender, RoutedEventArgs e)
        {
            selected_ptv.Clear();
            selected_ptv2.Clear();
        }

        private void clear_oar_Click(object sender, RoutedEventArgs e)
        {
            abcd.Items.Clear();
        }

        private void dup_button_Click(object sender, RoutedEventArgs e)
        {
            int w = 1;
            foreach (var xxx in abcd.Items)
            {
                vs1.Add(xxx.ToString());
                string tb = "tb"+ w.ToString();
                TextBox textBox = new TextBox();
                textBox.Name = tb;
                textBox.Width = 50;
                textBox.Height = 20;
                textBox.Margin = new Thickness(15, 1, 0, 0);
                textBox.Text = "3.0";
                PTVmargin.Children.Add(textBox);
                w++;

            }
            int OarCount;
            OarCount = vs1.Count();
            vs1.Add(selected_ptv.Text);
            if(selected_ptv2.Text==null)
            {vs1.Add(selected_ptv2.Text);}            
            Dup_struc(vs1, structureSet);
            MessageBox.Show("Congrats! Duplication complete.");
            marginFrmbody.Text = "3.0";
            int i = 0;
            while (i<OarCount)
            {
                dup_oar.Items.Add(vs2[i]);    //vs2 is the list of empty duplicated structures
                i++;
            }
            dup_ptv.Items.Add(vs2[i+1]);
            if (selected_ptv2.Text == null)
            { dup_ptv.Items.Add(vs2[i + 2]); }




            /*foreach(var x1 in vs2)  
            {
                if (x1 != selected_ptv.Text + "_d")
                { 
                    dup_oar.Items.Add(x1);
                }
                else
                { 
                    dup_ptv.Text = selected_ptv.Text + "_d";
                }
                
            }           */

        }

        private void crop_button_Click(object sender, RoutedEventArgs e)
        {
            Crop_PTV(double.Parse(marginFrmbody.Text),selected_ptv.Text);
            if (selected_ptv2.Text == null)
            { Crop_PTV(double.Parse(marginFrmbody.Text), selected_ptv2.Text); }
           
            MessageBox.Show("Duplicated structures cropped !!");

            foreach(var txtb in PTVmargin.Children.OfType<TextBox>())
            {
                vs4.Add(txtb.Text); //vs4 collecting oar cropping margins
            }
            if (selected_ptv2.Text == null)
            { vs4.Add(marginFrmbody.Text); }
            int ii = 0;
            foreach (var i in abcd.Items)
            {
                var xxxxx = vs4[ii];
                Crop_OAR(double.Parse(xxxxx), i.ToString());
                ii++;
            }
            Structure[] newS1 = structureSet.Structures.ToArray(); //to get updated structure set list
            cb.Items.Clear();
            foreach (var z in newS1)
            {
                cb.Items.Add(z.Id);
            }

        }

        private void rmv_struc_Click(object sender, RoutedEventArgs e)
        {
            foreach (Structure x in structureSet.Structures)
            {
                if (x.IsEmpty)
                { vs3.Add(x.Id); }
            }
            string yy = "";
            foreach (string y in vs3)
            {
                Structure S = structureSet.Structures.FirstOrDefault(s => s.Id.Equals(y));
                structureSet.RemoveStructure(S);
                yy = yy + y + "\n\n";
            }
            int c = vs3.Count;
            MessageBox.Show("Following " + c.ToString() + " empty structures were removed:\n\n" + yy);
        }






        // Following are the functions:



        public void Dup_struc(List<string> vs, StructureSet set)
        {
            foreach (var xxxx in vs)
            {                
                foreach (var y in set.Structures.ToArray())
                {
                    if (y.Name != "BODY" && y.Name == xxxx)
                    {                        
                        string yy = y.Name + "_d";
                        set.AddStructure(y.DicomType, yy);
                        vs2.Add(yy);
                    }
                    
                }
            }
            
            
        }         
        public void Crop_OAR(double mrgn, String stru_name)
        {
            double x;
            x = mrgn;

            Structure SS0 = structureSet.Structures.FirstOrDefault(s => s.Name.Equals(stru_name));            
            structureSet.AddStructure(SS0.DicomType,"dummy1");
            structureSet.AddStructure(SS0.DicomType,"dummy2");            
            Structure SS1 = structureSet.Structures.FirstOrDefault(s => s.Name.Equals("dummy1"));
            Structure SS2 = structureSet.Structures.FirstOrDefault(s => s.Name.Equals("dummy2"));
            Structure SS3 = structureSet.Structures.FirstOrDefault(s => s.Name.Equals(selected_ptv.Text));            
            SS1.SegmentVolume = SS3.SegmentVolume.Margin(x);
            SS2.SegmentVolume = SS1.SegmentVolume.Not();
            Structure SS = structureSet.Structures.FirstOrDefault(s => s.Name.Equals(stru_name + "_d"));
            SS.SegmentVolume = SS2.SegmentVolume.And(SS0.SegmentVolume);
            structureSet.RemoveStructure(SS1);
            structureSet.RemoveStructure(SS2);

        }


        public void Crop_PTV(double mrgn,string stru_name)
        {
            double x;
            x = -mrgn;

            Structure SS0 = structureSet.Structures.FirstOrDefault(s => s.Name.Equals("BODY"));
            structureSet.AddStructure("Organ", "dummy1");
            structureSet.AddStructure("Organ", "dummy2");
            Structure SS1 = structureSet.Structures.FirstOrDefault(s => s.Name.Equals("dummy1"));
            Structure SS2 = structureSet.Structures.FirstOrDefault(s => s.Name.Equals("dummy2"));
            SS1.SegmentVolume = SS0.SegmentVolume;
            SS2.SegmentVolume = SS1.SegmentVolume.Margin(x);
            Structure SS = structureSet.Structures.FirstOrDefault(s => s.Name.Equals(selected_ptv.Text));//here we should have stru_name
            Structure S = structureSet.Structures.FirstOrDefault(s => s.Name.Equals(dup_ptv.Text));
            S.SegmentVolume = SS2.SegmentVolume.And(SS.SegmentVolume);
            structureSet.RemoveStructure(SS1);
            structureSet.RemoveStructure(SS2);

        }

        public void Crop_ring(double mrgn, String stru_name)
        {
            double x;
            x = mrgn;

            Structure SS0 = structureSet.Structures.FirstOrDefault(s => s.Name.Equals(stru_name));
            structureSet.AddStructure(SS0.DicomType, "dummy1");
            structureSet.AddStructure(SS0.DicomType, "dummy2");
            Structure SS1 = structureSet.Structures.FirstOrDefault(s => s.Name.Equals("dummy1"));
            Structure SS2 = structureSet.Structures.FirstOrDefault(s => s.Name.Equals("dummy2"));
            Structure SS3 = structureSet.Structures.FirstOrDefault(s => s.Name.Equals(selected_strtr.Text));
            SS1.SegmentVolume = SS3.SegmentVolume.Margin(x);
            SS2.SegmentVolume = SS1.SegmentVolume.Not();
            Structure SS = structureSet.Structures.FirstOrDefault(s => s.Name.Equals("dummy_ring"));
            SS.SegmentVolume = SS2.SegmentVolume.And(SS0.SegmentVolume);
            structureSet.RemoveStructure(SS1);
            structureSet.RemoveStructure(SS2);

        }

        public void Crop_rg_fr_bdy(double mrgn, string stru_name)
        {
            double x;
            x = -mrgn;

            Structure SS0 = structureSet.Structures.FirstOrDefault(s => s.Name.Equals("BODY"));
            structureSet.AddStructure("Control", "dummy1");
            structureSet.AddStructure("Control", "dummy2");
            Structure SS1 = structureSet.Structures.FirstOrDefault(s => s.Name.Equals("dummy1"));
            Structure SS2 = structureSet.Structures.FirstOrDefault(s => s.Name.Equals("dummy2"));
            SS1.SegmentVolume = SS0.SegmentVolume; //ss1 is body
            SS2.SegmentVolume = SS1.SegmentVolume.Margin(x); // 
            structureSet.RemoveStructure(SS1);
            
            Structure SS = structureSet.Structures.FirstOrDefault(s => s.Name.Equals(stru_name));
            Structure S = structureSet.Structures.FirstOrDefault(s => s.Name.Equals(ring_name.Text));
            bool z = SS.IsHighResolution;            
            if (z)
            {
                /*MessageBox.Show(z.ToString());
                MessageBox.Show(z2.ToString());
                MessageBox.Show(z1.ToString());
                MessageBox.Show(SS.CanConvertToHighResolution().ToString());*/
                SS2.CanConvertToHighResolution();
                SS2.ConvertToHighResolution();

            }
            S.SegmentVolume = SS2.SegmentVolume.And(SS.SegmentVolume);
            structureSet.RemoveStructure(SS2);
            
            

        }





        // Ring Logic


        private void cb_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            selected_strtr.Text = cb.SelectedItem.ToString();
            
        }
        private void create_ring_Click(object sender, RoutedEventArgs e)
        {
            double in_mg;
            double ot_mg;
            ot_mg = double.Parse(outer_margin.Text);
            in_mg = double.Parse(inner_margin.Text);
            structureSet.AddStructure("Control",ring_name.Text);
            structureSet.AddStructure("Control", "dummy_ring");
            Structure SS1 = structureSet.Structures.FirstOrDefault(s => s.Name.Equals(ring_name.Text));
            Structure SS2 = structureSet.Structures.FirstOrDefault(s => s.Name.Equals("dummy_ring"));
            Structure SS3 = structureSet.Structures.FirstOrDefault(s => s.Name.Equals(selected_strtr.Text));
            SS1.SegmentVolume = SS3.SegmentVolume.Margin(ot_mg);            
            //bool y = SS1.IsHighResolution;
            //MessageBox.Show(y.ToString());
            Crop_ring(in_mg, SS1.Id);
            //double abcd;
            //abcd = 0.0;
            Crop_rg_fr_bdy(0.0,"dummy_ring");
            structureSet.RemoveStructure(SS2);
            MessageBox.Show("RING CREATED!");


            






        }

        

        
    }
}
