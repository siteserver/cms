interface UEditorInstance {
    setContent(html: string): void;
    getContent(): string;
    getContentTxt(): string;
    execCommand(cmd: string, options?: any);
    destroy(): void;
    ready(func: any): void;
    removeListener(eventName: string): void;
    addListener(eventName: string, func: (eventName: string) => void): void;
}

declare module UE {
    function getEditor(id: string, options?: any): UEditorInstance;
    function Editor(): void;
}
