# 创建动态库
add_library(NativeLib SHARED NativeLib.h NativeLib.cpp)

# 指定PLC头文件目录
target_include_directories(NativeLib PUBLIC ${CMAKE_CURRENT_SOURCE_DIR})