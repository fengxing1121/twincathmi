module TcHmi {
    export module Controls {
        export module TcHmiRiesControls {
            export class TcHmiSlideButton extends TcHmi.Controls.System.TcHmiControl {

                protected __currentState: boolean | undefined;
                protected __inputElement: JQuery;

                constructor(element: JQuery, pcElement: JQuery, attrs: TcHmi.Controls.ControlAttributeList) {
                    super(element, pcElement, attrs);
                }

                public __previnit() {
                    super.__previnit();
                }

                public __init() {
                    super.__init();
                }

                public __attach() {
                    super.__attach();                   

                    this.__inputElement = this.__element.find('input');
                    let __this = this;
                    this.__inputElement.click(function (e) {
                        __this.__currentState = __this.__inputElement.prop("checked");
                        TcHmi.EventProvider.raise(__this.__id + '.onFunctionResultChanged', ['getState']);
                    });
                }

                public __detach() {
                    super.__detach();
                }

                public destroy() {
                    super.destroy();
                }

                public setState(valueNew: boolean | null) {
                    let convertedValue = TcHmi.ValueConverter.toBoolean(valueNew);
                    if (convertedValue === null) {
                        convertedValue = this.getAttributeDefaultValueInternal('CurrentState') as boolean;
                    }
                    if (convertedValue === this.__currentState) {
                        return;
                    }
                    this.__currentState = convertedValue;
                    TcHmi.EventProvider.raise(this.__id + '.onFunctionResultChanged', ['getState']);
                    this.__processCurrentState();
                }

                public getState() {
                    return this.__currentState;
                }

                protected __processCurrentState() {
                    this.__inputElement = this.__element.find('input');
                    this.__inputElement.prop('checked', this.__currentState);
                }
            }
        }

        registerEx('TcHmiSlideButton', 'Ries.Hmi.Controls', TcHmiRiesControls.TcHmiSlideButton);
    }
}