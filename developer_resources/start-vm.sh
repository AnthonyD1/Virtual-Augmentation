#!/bin/bash
# This script will start up the virtual machine development environment
# Pass the path to the disk image as the first command-line argument
# You need to make sure VT-x or AMD-V is enabled in your BIOS settings
# You can set the amount of RAM in megabytes here:
RAM=2048

qemu-system-x86_64 \
	-drive media=disk,file="$1",format=qcow2,index=0 \
	-m $RAM \
	-cpu host \
	--enable-kvm \
	-usbdevice tablet \
	-vga qxl \
	-spice port=5900,addr=127.0.0.1,disable-ticketing \
	-device virtio-serial-pci \
	-device virtserialport,chardev=spicechannel0,name=com.redhat.spice.0 -chardev spicevmc,id=spicechannel0,name=vdagent
