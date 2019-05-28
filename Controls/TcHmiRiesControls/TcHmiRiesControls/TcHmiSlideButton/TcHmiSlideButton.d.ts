declare module TcHmi {
    module Controls {
        module TcHmiRiesControls {
            class TcHmiSlideButton extends TcHmi.Controls.System.TcHmiControl {
                protected __currentState: boolean | undefined;
                protected __inputElement: JQuery;
                constructor(element: JQuery, pcElement: JQuery, attrs: TcHmi.Controls.ControlAttributeList);
                __previnit(): void;
                __init(): void;
                __attach(): void;
                __detach(): void;
                destroy(): void;
                setState(valueNew: boolean | null): void;
                getState(): boolean | undefined;
                protected __processCurrentState(): void;
            }
        }
    }
}
