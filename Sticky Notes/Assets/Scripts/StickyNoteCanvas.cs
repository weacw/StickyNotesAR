using System.Collections.Generic;
using Unity.UIWidgets.engine;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.gestures;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;
using UnityEngine;
using Color = Unity.UIWidgets.ui.Color;

public class StickyNoteCanvas : WidgetCanvas
{
    protected override Widget getWidget()
    {
        return new StickyNoteHintStatefulWidget();
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        FontManager.instance.addFont(Resources.Load<Font>(path: "Material-Design-Iconic-Font"));
    }


    //protected override string initialRoute { get { return "/"; } }
    protected override Dictionary<string, WidgetBuilder> routes
    {
        get
        {
            return new Dictionary<string, WidgetBuilder>
            {
                {"/",(context)=>new StickyNoteLoginViewStatefulWidget() },
                {"StickyNoteView",(context)=>new StickyNoteStatefulWidget() },
                {"ARView",(context)=>new StickyNoteARViewStatefulWidget() },
                {"WriteView",(context)=>new StickyNoteWriterViewStatefulWidget() },
              //  {"FindPanelView",(context)=>new FrameAnimatfulWidgetion() }
            };
        }
    }

    protected override PageRouteFactory pageRouteBuilder
    {
        get
        {
            return (RouteSettings settings, WidgetBuilder builder) => new PageRouteBuilder(
                settings: settings,
                pageBuilder: (BuildContext context,
                              Unity.UIWidgets.animation.Animation<float> animation,
                              Unity.UIWidgets.animation.Animation<float> secondaryAnimation) => builder(context),
                transitionsBuilder: (BuildContext context,
                              Unity.UIWidgets.animation.Animation<float> animation,
                              Unity.UIWidgets.animation.Animation<float> secondaryAnimation, Widget child) =>
                new _FadeUpwardsPageTransition(
                                                routeAnimation: animation,
                                                child: child)
                );
        }
    }
}





public class CustomButton : StatelessWidget
{
    public CustomButton(
        Key key = null,
        GestureTapCallback onPressed = null,
        EdgeInsets padding = null,
        Color backgroundColor = null,
        Widget child = null
    ) : base(key: key)
    {
        this.onPressed = onPressed;
        this.padding = padding ?? EdgeInsets.all(8.0f);
        this.backgroundColor = backgroundColor ?? CLColors.transparent;
        this.child = child;
    }

    public readonly GestureTapCallback onPressed;
    public readonly EdgeInsets padding;
    public readonly Widget child;
    public readonly Color backgroundColor;

    public override Widget build(BuildContext context)
    {
        return new GestureDetector(
            onTap: this.onPressed,
            child: new Container(
                padding: this.padding,
                color: this.backgroundColor,
                child: this.child
            )
        );
    }
}
public static class Icons
{
    public static readonly IconData write = new IconData(0xf158, fontFamily: "Material-Design-Iconic-Font");
    public static readonly IconData back = new IconData(0x2039, fontFamily: "Material-Design-Iconic-Font");
    public static readonly IconData place = new IconData(0x2609, fontFamily: "Material-Design-Iconic-Font");
}
public static class CLColors
{
    public static readonly Color primary = new Color(0xFFE91E63);
    public static readonly Color secondary1 = new Color(0xFF00BCD4);
    public static readonly Color secondary2 = new Color(0xFFF0513C);
    public static readonly Color background1 = new Color(0xFF292929);
    public static readonly Color background2 = new Color(0xFF383838);
    public static readonly Color background3 = new Color(0xFFF5F5F5);
    public static readonly Color background4 = new Color(0xFF00BCD4);
    public static readonly Color icon1 = new Color(0xFFFFFFFF);
    public static readonly Color icon2 = new Color(0xFFA4A4A4);
    public static readonly Color text1 = new Color(0xFFFFFFFF);
    public static readonly Color text2 = new Color(0xFFD8D8D8);
    public static readonly Color text3 = new Color(0xFF959595);
    public static readonly Color text4 = new Color(0xFF002835);
    public static readonly Color text5 = new Color(0xFF9E9E9E);
    public static readonly Color text6 = new Color(0xFF002835);
    public static readonly Color text7 = new Color(0xFF5A5A5B);
    public static readonly Color text8 = new Color(0xFF239988);
    public static readonly Color text9 = new Color(0xFFB3B5B6);
    public static readonly Color text10 = new Color(0xFF00BCD4);
    public static readonly Color dividingLine1 = new Color(0xFF666666);
    public static readonly Color dividingLine2 = new Color(0xFF404040);

    public static readonly Color transparent = new Color(0x00000000);
    public static readonly Color white = new Color(0xFFFFFFFF);
    public static readonly Color black = new Color(0xFF000000);
    public static readonly Color red = new Color(0xFFFF0000);
    public static readonly Color green = new Color(0xFF00FF00);
    public static readonly Color blue = new Color(0xFF0000FF);

    public static readonly Color header = new Color(0xFF060B0C);
}
public class _FadeUpwardsPageTransition : StatelessWidget
{
    internal _FadeUpwardsPageTransition(
        Key key = null,
        Unity.UIWidgets.animation.Animation<float> routeAnimation = null, // The route's linear 0.0 - 1.0 animation.
        Widget child = null
    ) : base(key: key)
    {
        this._positionAnimation = _bottomUpTween.chain(_fastOutSlowInTween).animate(routeAnimation);
        this._opacityAnimation = _easeInTween.animate(routeAnimation);
        this.child = child;
    }

    static Unity.UIWidgets.animation.Tween<Offset> _bottomUpTween = new Unity.UIWidgets.animation.OffsetTween(
        begin: new Offset(0.0f, 0.25f),
        end: Offset.zero
    );

    static Unity.UIWidgets.animation.Animatable<float> _fastOutSlowInTween = new Unity.UIWidgets.animation.CurveTween(curve: Unity.UIWidgets.animation.Curves.easeOut);
    static Unity.UIWidgets.animation.Animatable<float> _easeInTween = new Unity.UIWidgets.animation.CurveTween(curve: Unity.UIWidgets.animation.Curves.easeIn);

    readonly Unity.UIWidgets.animation.Animation<Offset> _positionAnimation;
    readonly Unity.UIWidgets.animation.Animation<float> _opacityAnimation;
    public readonly Widget child;

    public override Widget build(BuildContext context)
    {
        return new SlideTransition(
            position: this._positionAnimation,
            child: new FadeTransition(
                opacity: this._opacityAnimation,
                child: this.child
            )
        );
    }
}
[System.Serializable]
public class Item
{
    public string title;
    public string date;
    public Item(string item, string date)
    {
        this.title = item;
        this.date = date;
    }
}
[System.Serializable]
public class ItemList
{
    public List<Item> tmp = new List<Item>();
}

public class ScreenHelper
{
    private static  MediaQueryData mediaQueryData;
    public static Vector2 _getScreenSize(BuildContext context)
    {
        if (mediaQueryData == null) mediaQueryData = MediaQuery.of(context);
        return new Vector2(mediaQueryData.size._dx, mediaQueryData.size._dy);
    }
}