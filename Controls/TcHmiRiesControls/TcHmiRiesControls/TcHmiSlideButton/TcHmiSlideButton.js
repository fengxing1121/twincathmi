var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var TcHmi;
(function (TcHmi) {
    var Controls;
    (function (Controls) {
        var TcHmiRiesControls;
        (function (TcHmiRiesControls) {
            var TcHmiSlideButton = /** @class */ (function (_super) {
                __extends(TcHmiSlideButton, _super);
                function TcHmiSlideButton(element, pcElement, attrs) {
                    return _super.call(this, element, pcElement, attrs) || this;
                }
                TcHmiSlideButton.prototype.__previnit = function () {
                    _super.prototype.__previnit.call(this);
                };
                TcHmiSlideButton.prototype.__init = function () {
                    _super.prototype.__init.call(this);
                };
                TcHmiSlideButton.prototype.__attach = function () {
                    _super.prototype.__attach.call(this);
                    this.__inputElement = this.__element.find('input');
                    var __this = this;
                    this.__inputElement.click(function (e) {
                        __this.__currentState = __this.__inputElement.prop("checked");
                        TcHmi.EventProvider.raise(__this.__id + '.onFunctionResultChanged', ['getState']);
                    });
                };
                TcHmiSlideButton.prototype.__detach = function () {
                    _super.prototype.__detach.call(this);
                };
                TcHmiSlideButton.prototype.destroy = function () {
                    _super.prototype.destroy.call(this);
                };
                TcHmiSlideButton.prototype.setState = function (valueNew) {
                    var convertedValue = TcHmi.ValueConverter.toBoolean(valueNew);
                    if (convertedValue === null) {
                        convertedValue = this.getAttributeDefaultValueInternal('CurrentState');
                    }
                    if (convertedValue === this.__currentState) {
                        return;
                    }
                    this.__currentState = convertedValue;
                    TcHmi.EventProvider.raise(this.__id + '.onFunctionResultChanged', ['getState']);
                    this.__processCurrentState();
                };
                TcHmiSlideButton.prototype.getState = function () {
                    return this.__currentState;
                };
                TcHmiSlideButton.prototype.__processCurrentState = function () {
                    this.__inputElement = this.__element.find('input');
                    this.__inputElement.prop('checked', this.__currentState);
                };
                return TcHmiSlideButton;
            }(TcHmi.Controls.System.TcHmiControl));
            TcHmiRiesControls.TcHmiSlideButton = TcHmiSlideButton;
        })(TcHmiRiesControls = Controls.TcHmiRiesControls || (Controls.TcHmiRiesControls = {}));
        Controls.registerEx('TcHmiSlideButton', 'Ries.Hmi.Controls', TcHmiRiesControls.TcHmiSlideButton);
    })(Controls = TcHmi.Controls || (TcHmi.Controls = {}));
})(TcHmi || (TcHmi = {}));
//# sourceMappingURL=TcHmiSlideButton.js.map