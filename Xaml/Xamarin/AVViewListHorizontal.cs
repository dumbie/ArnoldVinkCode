using System;
using System.Diagnostics;
using Xamarin.Forms;

namespace ArnoldVinkXaml
{
    public class ListViewHorizontal : ListView
    {


        public Int32 WidthTest { get; set; }


        public ListViewHorizontal()
        {
            try
            {

                //Add the ContentView wrap

                //Rotate the listview
                this.Rotation = 270;


                Debug.WriteLine(this.WidthTest);


                Debug.WriteLine(this.WidthRequest);
            }
            catch { }

            // <RelativeLayout HeightRequest="60">
            // <ListView Rotatation="270" ItemSource="{Binding Items}" RowHeight="60" SeparatorVisibility="None"
            // RelativeLayout.XConstraint="{ConstraintExpression Type=RelativeToParent, Property=Width, Factor=0.5, Constant=-30}"
            // RelativeLayout.YConstraint="{ConstraintExpression Type=RelativeToParent, Property=Width, Factor=-0.5, Constant=30}"
            // RelativeLayout.WidthConstraint="{ConstraintExpression Type=Constant, Constant=60}"
            // RelativeLayout.HeightConstraint="{ConstraintExpression Type=RelativeToParent, Property=Width, Factor=1}">
            //  <ListView.ItemTemplate>
            //   <DataTemplate>
            //    <ViewCell>
            //     <ContentView Rotatation="90" Padding="1" TranslationX="60">
            //      <Image Source="{Binding Source}" HeightRequest="58" WidthRequest="58" Aspect="AspectFill"></Image>
            //     </ContentView>
            //    </ViewCell>
            //   </DataTemplate>
            //  </ListView.ItemTemplate>
            // </ListView>
            //</RelativeLayout>


            //this.BackgroundColor = Color.Red;
        }
    }
}