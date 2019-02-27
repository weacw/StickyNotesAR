using System.Collections.Generic;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.material;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;
using UnityEngine.XR.ARFoundation;
using Color = Unity.UIWidgets.ui.Color;
using TextStyle = Unity.UIWidgets.painting.TextStyle;

#region StickyNote Writer view
public class StickyNoteWriterViewStatefulWidget : StatefulWidget
{
    public StickyNoteWriterViewStatefulWidget(Key key = null) : base(key)
    {
    }

    public override State createState()
    {
        return new StickyNoteWriterViewState();
    }
}

public class StickyNoteWriterViewState : State<StickyNoteWriterViewStatefulWidget>
{
    TextEditingController descController = new TextEditingController("");

    public override Widget build(BuildContext context)
    {

        var container = new Container(
            color: CLColors.background3,
            child: new Column(
                    children: new List<Widget>
                    {
                        this._buildHeader(context),
                        this._buildInputArea(context)
                    }
                )
            );
        return container;
    }


    private Widget _buildHeader(BuildContext context)
    {
        return new Column(
                  children: new List<Widget>
                  {
                      new Container(
                            margin:EdgeInsets.only(left:ScreenHelper._getScreenSize(context).x*0.75f,top:ScreenHelper._getScreenSize(context).y*0.05f),
                            child:new GestureDetector(
                                    onTap:()=>{
                                        Navigator.popAndPushNamed(context,"ARView");

#if UNITY_IOS
                                        ARSubsystemManager.CreateSubsystems();
                                        ARSubsystemManager.StartSubsystems();
#endif

                                        NotifCenterSystem.GetNotice.PostDispatchEvent(NotifEventKeyComponent.SetInputStringToContentText,
                                        new AssingObject<string>(descController.text));


                                  },
                                    child:new Text("Finished", style: new TextStyle(fontSize: 20,color:CLColors.blue))
                                )
                          ),
                      new Container(
                            margin:EdgeInsets.only(right:ScreenHelper._getScreenSize(context).x*0.75f,top:20f),
                            child:new Text("Writting", style: new TextStyle(fontSize: 20,fontWeight: FontWeight.w700))
                          )
                  }
          );
    }

    private Widget _buildInputArea(BuildContext context)
    {
        return new Container(
                child: new Column(
                        children: new List<Widget>
                        {
                            new Flexible(child:new Container(
                                   margin:EdgeInsets.all(ScreenHelper._getScreenSize(context).x*0.025f),
                                    decoration:new BoxDecoration(border:Border.all(new Color(0xFF000000),1)),
                                    child:new EditableText(maxLines:20,
                                        controller:this.descController,
                                        selectionControls:MaterialUtils.materialTextSelectionControls,
                                        focusNode:new FocusNode(),
                                        style:new TextStyle(
                                                fontSize:15,
                                                height:1.5f,
                                                color:new Color(0xFF1389FD)
                                            ),
                                        selectionColor:Color.fromARGB(255,255,0,0),
                                        cursorColor:Color.fromARGB(255,0,0,0))
                                    )
                                )
                        }
                    )
            );
    }
}
#endregion

