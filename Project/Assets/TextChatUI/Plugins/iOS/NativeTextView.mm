@interface KeyboardNotification: NSObject
@property float height;
@property bool isShow;
-(void)setup;
-(void)keyboardWillShow:(NSNotification*)note;
-(void)keyboardWillHide:(NSNotification*)note;
-(float)getHeight;
-(bool)getIsShow;
@end

@implementation KeyboardNotification

-(void)setup{
    [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(keyboardWillShow:) name:UIKeyboardWillShowNotification object:nil];
    [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(keyboardWillHide:) name:UIKeyboardWillHideNotification object:nil];
}

-(void)keyboardWillShow:(NSNotification *)note
{
    CGRect keyboardFrameEnd = [[note.userInfo objectForKey:UIKeyboardFrameEndUserInfoKey] CGRectValue];
    self.height = keyboardFrameEnd.size.height;
    self.isShow = true;
}

-(void)keyboardWillHide:(NSNotification *)note
{
    self.height = 0.0f;
    self.isShow = false;
}

-(float)getHeight
{
    return self.height;
}

-(bool)getIsShow
{
    return self.isShow;
}

@end

extern "C"
{
    UITextView* textView = NULL;
    bool isShowTextView = false;
    float hideHeight = 0.0f;
    KeyboardNotification* keyboardNotification = NULL;

    void createTextView_();
    void updateTextViewRect_(float origin_x, float origin_y, float width, float height);
    void clearTextViewText_();
    void getTextViewText_(char* str_p);
    int getTextViewTextCount_(bool isSpaceOmit);
    int getTextViewTextByteCount_();
    void setTextViewFont_(float size, float color_r, float color_g, float color_b, float color_a);
    float getTextViewHeight_();
    void showTextView_();
    void hideTextView_();
    void becomeFirstResponderTextView_();
    void resignFirstResponderTextView_();
    float getViewHeight_();
    float getViewWidth_();
    float getKeyboardHeight_();
    bool getKeyboardIsShow_();
}

extern UIViewController* UnityGetGLViewController();

void createTextView_() {
    if(textView == NULL)
    {
        UIViewController* viewController = UnityGetGLViewController();
        textView = [[UITextView alloc] init];
        UIColor *color = [UIColor whiteColor];
        UIColor *acolor = [color colorWithAlphaComponent:0.0];
        textView.backgroundColor = acolor;
        textView.frame = CGRectMake(0, 0, 0, 0);
        textView.font = [UIFont systemFontOfSize:14];
        textView.bounces = false;
        textView.textContainerInset = UIEdgeInsetsZero;
        textView.textContainer.lineFragmentPadding = 0;
        [viewController.view addSubview:textView];
        isShowTextView = true;
        keyboardNotification = [[KeyboardNotification alloc] init];
        [keyboardNotification setup];
    }
}

void updateTextViewRect_(float origin_x, float origin_y, float width, float height)
{
    if(textView != NULL)
    {
        textView.frame = CGRectMake(origin_x, origin_y, width, height);
    }
}

void clearTextViewText_()
{
    if(textView != NULL)
    {
        hideHeight = 0.0f;
        textView.text = NULL;
    }
}

void getTextViewText_(char* str_p)
{
    int byteLength = getTextViewTextByteCount_();
    if(byteLength == 0) { return; }
    const char* str = [textView.text UTF8String];
    memcpy(str_p, str, strlen(str));
    str_p[strlen(str)] = '\0';
}

int getTextViewTextCount_(bool isSpaceOmit)
{
    if(textView != NULL && textView.text != NULL)
    {
        if(!isSpaceOmit){ return (int)textView.text.length; }
        for(int i = 0; i < textView.text.length; i++)
        {
            NSString* str = [textView.text substringWithRange:NSMakeRange(i, 1)];
            if([str isEqualToString:@"\n"]){ continue; }
            if([str isEqualToString:@" "]){ continue; }
            return (int)textView.text.length;
        }
    }

    return 0;
}

int getTextViewTextByteCount_()
{
    if(textView != NULL)
    {
        if(textView.text != NULL && textView.text.length > 0)
        {
            const char* str = [textView.text UTF8String];
            return (int)((sizeof(char) * (strlen(str) + 1)));
        }
    }

    return 0;
}

void setTextViewFont_(float size, float color_r, float color_g, float color_b, float color_a)
{
    if(textView != NULL)
    {
        textView.font = [UIFont systemFontOfSize:size];
        textView.textColor = [UIColor colorWithRed:color_r green:color_g blue:color_b alpha:color_a];
    }
}

float getTextViewHeight_()
{
    if(textView != NULL)
    {
        if(isShowTextView) { return textView.contentSize.height; }
        else { return hideHeight; }
    }

    return 0.0f;
}

void showTextView_()
{
    if(textView != NULL && !isShowTextView)
    {
        UIViewController* viewController = UnityGetGLViewController();
        [viewController.view addSubview:textView];
        isShowTextView = true;
    }
}

void hideTextView_()
{
    if(textView != NULL && isShowTextView)
    {
        if(textView.text == NULL) { hideHeight = 0.0f; }
        else { hideHeight = getTextViewHeight_(); }
        [textView removeFromSuperview];
        isShowTextView = false;
    }
}

void becomeFirstResponderTextView_()
{
    if(textView != NULL)
    {
        showTextView_();
        [textView becomeFirstResponder];
    }
}

void resignFirstResponderTextView_()
{
    if(textView != NULL)
    {
        [textView resignFirstResponder];
    }
}

float getViewHeight_()
{
    UIViewController* viewController = UnityGetGLViewController();
    return viewController.view.bounds.size.height;
}

float getViewWidth_()
{
    UIViewController* viewController = UnityGetGLViewController();
    return viewController.view.bounds.size.width;
}

float getKeyboardHeight_()
{
    if(keyboardNotification != NULL)
    {
        float keyboardHeight = [keyboardNotification getHeight];
        return keyboardHeight;
    }
    
    return 0.0f;
}

bool getKeyboardIsShow_()
{
    if(keyboardNotification != NULL)
    {
        bool isShowKeyboard = [keyboardNotification getIsShow];
        return isShowKeyboard;
    }
    
    return false;
}
