module MessageCapabilitiesTypes

/// MODE: Someone gained or lost operator:
/// :jtv MODE #channel +o operator_user
type MessageMembershipMode = {
    Channel:string    
    /// 1: +o, 0: -o
    Mode:bool    
    Username:string}


