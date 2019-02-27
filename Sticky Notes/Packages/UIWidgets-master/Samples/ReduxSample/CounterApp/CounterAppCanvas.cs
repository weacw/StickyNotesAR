using System;
using System.Collections.Generic;
using Unity.UIWidgets.engine;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.gestures;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;
using TextStyle = Unity.UIWidgets.painting.TextStyle;

namespace Unity.UIWidgets.Sample.Redux {
    public class CounterAppCanvas : WidgetCanvas {
        protected override Widget getWidget() {
            return new CounterApp();
        }
    }

    [Serializable]
    public class CouterState {
        public int count;

        public CouterState(int count = 0) {
            this.count = count;
        }
    }

    [Serializable]
    public class CounterIncAction {
        public int amount;
    }

    public class CounterApp : StatelessWidget {
        public override Widget build(BuildContext context) {
            var store = new Store<CouterState>(reduce, new CouterState(),
                ReduxLogging.Create<CouterState>());
            return new StoreProvider<CouterState>(store, this.createWidget());
        }

        public static CouterState reduce(CouterState state, object action) {
            var inc = action as CounterIncAction;
            if (inc == null) {
                return state;
            }

            return new CouterState(inc.amount + state.count);
        }

        Widget createWidget() {
            return new Container(
                height: 200.0f,
                padding: EdgeInsets.all(10),
                decoration: new BoxDecoration(
                    color: new Color(0xFF7F7F7F),
                    border: Border.all(color: Color.fromARGB(255, 255, 0, 0), width: 5),
                    borderRadius: BorderRadius.all(2)),
                child: new Column(
                    children: new List<Widget>() {
                        new StoreConnector<CouterState, string>(
                            converter: (state, dispatch) => $"Count:{state.count}",
                            builder: (context, countText) => new Text(countText, style: new TextStyle(
                                fontSize: 20, fontWeight: FontWeight.w700
                            ))
                        ),
                        new StoreConnector<CouterState, Action>(
                            converter: (state, dispatch) => () => { dispatch(new CounterIncAction() {amount = 1}); },
                            builder: (context, onPress) => new CustomButton(
                                backgroundColor: Color.fromARGB(255, 0, 204, 204),
                                padding: EdgeInsets.all(10),
                                child: new Text("Add", style: new TextStyle(
                                    fontSize: 16, color: Color.fromARGB(255, 255, 255, 255)
                                )), onPressed: () => { onPress(); })
                        ),
                    }
                )
            );
        }
    }

    public class CustomButton : StatelessWidget {
        public CustomButton(
            Key key = null,
            GestureTapCallback onPressed = null,
            EdgeInsets padding = null,
            Color backgroundColor = null,
            Widget child = null
        ) : base(key: key) {
            this.onPressed = onPressed;
            this.padding = padding ?? EdgeInsets.all(8.0f);
            this.backgroundColor = backgroundColor;
            this.child = child;
        }

        public readonly GestureTapCallback onPressed;
        public readonly EdgeInsets padding;
        public readonly Widget child;
        public readonly Color backgroundColor;

        public override Widget build(BuildContext context) {
            return new GestureDetector(
                onTap: this.onPressed,
                child: new Container(
                    padding: this.padding,
                    decoration: new BoxDecoration(color: this.backgroundColor),
                    child: this.child
                )
            );
        }
    }
}