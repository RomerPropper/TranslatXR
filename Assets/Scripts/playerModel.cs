using System;
using Normal.Realtime;
using Normal.Realtime.Serialization;

namespace Normal.Realtime
{
    [RealtimeModel]
    public partial class playerModel
    {
        // Property 1 bitflag transitioned to 4/5/6 bools
        [RealtimeProperty(2, true)] private player.DeviceType _deviceType;
        [RealtimeProperty(3, true)] private string _deviceModel;
        [RealtimeProperty(4, true, true)] private bool _headActive;
        [RealtimeProperty(5, true, true)] private bool _leftHandActive;
        [RealtimeProperty(6, true, true)] private bool _rightHandActive;
    }
}

#if !UNITY_2021_1_OR_NEWER
namespace Normal.Realtime {
    public partial class playerModel : RealtimeModel {
        public Normal.Realtime.player.DeviceType deviceType {
            get {
                return (Normal.Realtime.player.DeviceType)_deviceTypeProperty.value;
            }
            set {
                if (_deviceTypeProperty.value == (uint)value) return;
                _deviceTypeProperty.value = (uint)value;
                InvalidateReliableLength();
            }
        }

        public string deviceModel {
            get {
                return _deviceModelProperty.value;
            }
            set {
                if (_deviceModelProperty.value == value) return;
                _deviceModelProperty.value = value;
                InvalidateReliableLength();
            }
        }

        public bool headActive {
            get {
                return _headActiveProperty.value;
            }
            set {
                if (_headActiveProperty.value == value) return;
                _headActiveProperty.value = value;
                InvalidateReliableLength();
                FireHeadActiveDidChange(value);
            }
        }

        public bool leftHandActive {
            get {
                return _leftHandActiveProperty.value;
            }
            set {
                if (_leftHandActiveProperty.value == value) return;
                _leftHandActiveProperty.value = value;
                InvalidateReliableLength();
                FireLeftHandActiveDidChange(value);
            }
        }

        public bool rightHandActive {
            get {
                return _rightHandActiveProperty.value;
            }
            set {
                if (_rightHandActiveProperty.value == value) return;
                _rightHandActiveProperty.value = value;
                InvalidateReliableLength();
                FireRightHandActiveDidChange(value);
            }
        }

        public delegate void PropertyChangedHandler<in T>(playerModel model, T value);
        public event PropertyChangedHandler<bool> headActiveDidChange;
        public event PropertyChangedHandler<bool> leftHandActiveDidChange;
        public event PropertyChangedHandler<bool> rightHandActiveDidChange;

        public enum PropertyID : uint {
            DeviceType = 2,
            DeviceModel = 3,
            HeadActive = 4,
            LeftHandActive = 5,
            RightHandActive = 6,
        }

#region Properties

        private ReliableProperty<uint> _deviceTypeProperty;

        private ReliableProperty<string> _deviceModelProperty;

        private ReliableProperty<bool> _headActiveProperty;

        private ReliableProperty<bool> _leftHandActiveProperty;

        private ReliableProperty<bool> _rightHandActiveProperty;

#endregion

        public playerModel() : base(null) {
            _deviceTypeProperty = new ReliableProperty<uint>(2, (uint)_deviceType);
            _deviceModelProperty = new ReliableProperty<string>(3, _deviceModel);
            _headActiveProperty = new ReliableProperty<bool>(4, _headActive);
            _leftHandActiveProperty = new ReliableProperty<bool>(5, _leftHandActive);
            _rightHandActiveProperty = new ReliableProperty<bool>(6, _rightHandActive);
        }

        protected override void OnParentReplaced(RealtimeModel previousParent, RealtimeModel currentParent) {
            _deviceTypeProperty.UnsubscribeCallback();
            _deviceModelProperty.UnsubscribeCallback();
            _headActiveProperty.UnsubscribeCallback();
            _leftHandActiveProperty.UnsubscribeCallback();
            _rightHandActiveProperty.UnsubscribeCallback();
        }

        private void FireHeadActiveDidChange(bool value) {
            try {
                headActiveDidChange?.Invoke(this, value);
            } catch (System.Exception exception) {
                UnityEngine.Debug.LogException(exception);
            }
        }

        private void FireLeftHandActiveDidChange(bool value) {
            try {
                leftHandActiveDidChange?.Invoke(this, value);
            } catch (System.Exception exception) {
                UnityEngine.Debug.LogException(exception);
            }
        }

        private void FireRightHandActiveDidChange(bool value) {
            try {
                rightHandActiveDidChange?.Invoke(this, value);
            } catch (System.Exception exception) {
                UnityEngine.Debug.LogException(exception);
            }
        }

        protected override int WriteLength(StreamContext context) {
            var length = 0;
            length += _deviceTypeProperty.WriteLength(context);
            length += _deviceModelProperty.WriteLength(context);
            length += _headActiveProperty.WriteLength(context);
            length += _leftHandActiveProperty.WriteLength(context);
            length += _rightHandActiveProperty.WriteLength(context);
            return length;
        }

        protected override void Write(WriteStream stream, StreamContext context) {
            var writes = false;
            writes |= _deviceTypeProperty.Write(stream, context);
            writes |= _deviceModelProperty.Write(stream, context);
            writes |= _headActiveProperty.Write(stream, context);
            writes |= _leftHandActiveProperty.Write(stream, context);
            writes |= _rightHandActiveProperty.Write(stream, context);
            if (writes) InvalidateContextLength(context);
        }

        protected override void Read(ReadStream stream, StreamContext context) {
            var anyPropertiesChanged = false;
            while (stream.ReadNextPropertyID(out uint propertyID)) {
                var changed = false;
                switch (propertyID) {
                    case (uint)PropertyID.DeviceType: {
                        changed = _deviceTypeProperty.Read(stream, context);
                        break;
                    }
                    case (uint)PropertyID.DeviceModel: {
                        changed = _deviceModelProperty.Read(stream, context);
                        break;
                    }
                    case (uint)PropertyID.HeadActive: {
                        changed = _headActiveProperty.Read(stream, context);
                        if (changed) FireHeadActiveDidChange(headActive);
                        break;
                    }
                    case (uint)PropertyID.LeftHandActive: {
                        changed = _leftHandActiveProperty.Read(stream, context);
                        if (changed) FireLeftHandActiveDidChange(leftHandActive);
                        break;
                    }
                    case (uint)PropertyID.RightHandActive: {
                        changed = _rightHandActiveProperty.Read(stream, context);
                        if (changed) FireRightHandActiveDidChange(rightHandActive);
                        break;
                    }
                    default: {
                        stream.SkipProperty();
                        break;
                    }
                }
                anyPropertiesChanged |= changed;
            }
            if (anyPropertiesChanged) {
                UpdateBackingFields();
            }
        }

        private void UpdateBackingFields() {
            _deviceType = deviceType;
            _deviceModel = deviceModel;
            _headActive = headActive;
            _leftHandActive = leftHandActive;
            _rightHandActive = rightHandActive;
        }
    }
}
#endif
