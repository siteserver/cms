///<reference path="../react/react.d.ts" />
///<reference path="../moment/moment.d.ts" />

declare module "react-datetime" {
    import React = __React;

    interface ReactDateTimeProps {
      value?: string | Date;
      defaultValue?: Date;
      dateFormat?: boolean | string;
      timeFormat?: boolean | string;
      open?: boolean;
      input?: boolean;
      locale?: string;
      onChange?(date?:moment.Moment):any;
      onFocus?():any;
      onBlur?():any;
      viewMode?: string | number;
      className?: string;
    }

    class ReactDatetime extends React.Component<ReactDateTimeProps, {}> {
    }

    export = ReactDatetime;
}
