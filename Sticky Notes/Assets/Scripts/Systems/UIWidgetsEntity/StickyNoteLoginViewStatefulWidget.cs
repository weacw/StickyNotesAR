using System.Collections.Generic;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;
using TextStyle = Unity.UIWidgets.painting.TextStyle;
using Image = Unity.UIWidgets.widgets.Image;


#region StickyNote Login view
public class StickyNoteLoginViewStatefulWidget : StatefulWidget
{

    public StickyNoteLoginViewStatefulWidget(Key key = null) : base(key)
    {
    }

    public override State createState()
    {
        return new StickyNoteLoginViewState();
    }
}

public class StickyNoteLoginViewState : State<StickyNoteLoginViewStatefulWidget>
{
    public override Widget build(BuildContext context)
    {
        return new Container(
                color: CLColors.white,
                child: new Column(
                    crossAxisAlignment: Unity.UIWidgets.rendering.CrossAxisAlignment.center,
                    mainAxisAlignment: Unity.UIWidgets.rendering.MainAxisAlignment.center,
                        children: new List<Widget>
                        {
                            //TODO:图片-logo
                            //TODO:login button
                            new Container(
                                width:200,
                                height:200,
                                padding:EdgeInsets.only(top:50),
                                child:Image.asset("StandardAssets/UIImage/StickyNoteIcon",fit:BoxFit.cover)
                            ),

                            new GestureDetector(
                                onTap:()=>{
                                    //Navigator.pushName(context,"StickyNoteView");
                                    Navigator.popAndPushNamed(context,"StickyNoteView");
                                  },
                                child: new Container(
                                        width:100,
                                        height:30,
                                        margin:EdgeInsets.only(top:256),
                                        decoration:new BoxDecoration(borderRadius:BorderRadius.circular(15),color:CLColors.black,shape:BoxShape.rectangle),
                                        child:new Container(
                                                margin:EdgeInsets.only(top:10),
                                                child:new Text("Login",style:new TextStyle(color:CLColors.white,fontSize:15),textAlign:TextAlign.center)
                                            )
                                    )
                            ),

                        }
                    )

            );
    }
}
#endregion
